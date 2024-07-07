using System.Collections;
using Bogus;
using Bogus.DataSets;
using WASM.Models;
using WASM.Utils;
using Random = System.Random;

namespace WASM.Runner;

//https://www.ncbi.nlm.nih.gov/pmc/articles/PMC9578784/table/TB2000172-1/?report=objectonly

public class SimulationRunner
{
    public double AverageAge { get; set; }
    public int MaxAge { get; set; }
    public int MinAge { get; set; }
    public long LivePersons { get; private set; }
    public long DeceasedPersons { get; private set; }
    public long PersonsAllTime { get; private set; }
    public long GeneticAbnormalities { get; private set; }
    public long Inbred { get; private set; }
    private bool Active { get; set; }

    private List<IPerson> Persons { get; } = new ();

    public Dictionary<int, List<Person>> PersonsByAge = new ();

    private Random Random { get; } = new ();

    public string LastName => Persons.LastOrDefault()?.Name ?? string.Empty;
    
    public void Start(SimulationConfig config)
    {
        Initialize(config);
        Task task = new Task(() => { Work(null); });
        task.Start();
    }

    private void Initialize(SimulationConfig config)
    {
        Persons.Clear();
        PersonsAllTime = GeneticAbnormalities = LivePersons = DeceasedPersons = Inbred = 0;

        var root = new RootParent();
        
        int initialPopulation = config.InitPopulation;
        Persons.AddRange(Enumerable.Range(0, initialPopulation - config.InitDonors).Select(x =>
            FabricatePerson(root)).ToList());

        for (int i = 0; i < config.InitDonors; i++)
        {
            var faker = new Faker();
            Persons.Add(new SpermDonor
            {
                Age = 17,
                Mother = root,
                Father = root,
                FirstName = faker.Name.FirstName(Name.Gender.Male),
                LastName = faker.Name.LastName(Name.Gender.Male)
            });         
        }
        
        LivePersons = initialPopulation;
        PersonsAllTime = initialPopulation;
        Active = true;
    }

    private IPerson FabricatePerson(IPerson root)
    {
        var person = new Person();
        return person.Gender == Name.Gender.Male
            ? new MalePerson
            {
                FirstName = person.FirstName,
                LastName = person.LastName,
                Age = 17,
                Mother = root,
                Father = root
            }
            : new FemalePerson
            {
                FirstName = person.FirstName,
                LastName = person.LastName,
                Age = 17,
                Mother = root,
                Father = root
            };
    }

    public void Stop()
    {
        Active = false;
    }

    private void Mate(MalePerson male, FemalePerson female)
    {
        //var mate = men.First(m => m.Age >= 17 && !set.Contains(m)); //todo add shuffle
        if (Roller.Roll(50))
        {
            var offspring = (BasePerson)(female).Mate(male);
            if (offspring != null)
            {
                Persons.Add(offspring);
                LivePersons++;
                PersonsAllTime++;
                if (offspring.Inbred)
                {
                    ++Inbred;
                }
                if (offspring.Abnormality)
                {
                    ++GeneticAbnormalities;
                }
            }
        }
    }
    
    private async void Work(object state)
    {
        while (Active)
        {
            await Task.Delay(500);
            
            //HashSet<IPerson> set = new HashSet<IPerson>();
            Dictionary<IPerson, int> MateCount = new Dictionary<IPerson, int>();

            List<int> deceasedIndices = new List<int>();
            
            int n = Persons.Count;
            while (n > 1)
            {
                n--;
                IPerson current = Persons[n--];
                IPerson other = Persons[Random.Next(0, n)];

                if (CanMate(current, other))
                {
                    if (current is MalePerson mp)
                    {
                        Mate(mp, (FemalePerson)other);
                    }
                    else if (current is FemalePerson fp)
                    {
                        Mate((MalePerson)other, fp);
                    }
                }
                int curAge = ((BasePerson)current).Age++; //increase age
                
                bool death = Roller.Roll(curAge);
                if (death)
                {
                    deceasedIndices.Add(n);
                }

            }

            foreach (int removal in deceasedIndices)
            {
                Persons.RemoveAt(removal);
                DeceasedPersons++;
            }
            
        }
    }
    
    private bool CanMate(IPerson a, IPerson b)
    {
        if (a.MateCount > 5 && !(a is SpermDonor))
        {
            return false;
        }

        if (b.MateCount > 5 && !(b is SpermDonor))
        {
            return false;
        }
        
        if (a.Age >= 17 && b.Age >= 17)
        {
            return a.Gender != b.Gender;
        }
        return false;
    }
}