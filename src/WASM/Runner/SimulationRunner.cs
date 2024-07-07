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
    public bool Active { get; private set; } = false;
    public SimulationMetrics Metrics { get; private set; }= new ();
    private List<IPerson> Persons { get; } = new ();

    public Dictionary<int, List<Person>> PersonsByAge = new ();

    private Random Random { get; } = new ();
    
    
    public void Start(SimulationConfig config)
    {
        Initialize(config);
        Task task = new Task(() => { Work(null); });
        task.Start();
    }

    private void Initialize(SimulationConfig config)
    {
        Persons.Clear();
        Metrics = new SimulationMetrics
        {
            InitDonors = config.InitDonors,
            InitMale = config.InitMalePopulation,
            InitFemale = config.InitFemalePopulation
        };
        
        var root = new RootParent();
        
        Persons.AddRange(Enumerable.Range(1, config.InitMalePopulation).Select(x =>
            FabricatePerson(root, Name.Gender.Male)).ToList());
        Persons.AddRange(Enumerable.Range(1, config.InitFemalePopulation).Select(x =>
            FabricatePerson(root, Name.Gender.Female)).ToList());

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
        
        Metrics.LivePersons = config.InitDonors + config.InitMalePopulation + config.InitFemalePopulation;
        Metrics.PersonsAllTime = Metrics.LivePersons;
        Active = true;
    }

    private IPerson FabricatePerson(IPerson root, Name.Gender gender)
    {
        Faker faker = new Faker();

        var ret = NameGenerator.GetName(gender);
        return gender == Name.Gender.Male
            ? new MalePerson
            {
                FirstName = ret.FirstName,
                LastName = ret.LastName,
                Age = 18,
                Mother = root,
                Father = root
            }
            : new FemalePerson
            {
                FirstName = ret.FirstName,
                LastName = ret.LastName,
                Age = 18,
                Mother = root,
                Father = root
            };
    }

    public void Stop()
    {
        Active = false;
    }

    private bool Mate(MalePerson male, FemalePerson female, long tick)
    {
        if (Roller.Roll(50)) //roll for successful mating
        {
            var offspring = (BasePerson)(female).Mate(male, tick);
            if (offspring != null)
            {
                Persons.Add(offspring);
                
                Metrics.Births++;
                Metrics.LivePersons++;
                Metrics.PersonsAllTime++;
                
                //TODO: add lookback up tree logic N parents
                if (offspring.Inbred)
                {
                    ++Metrics.Inbred;
                }
                if (offspring.Abnormality)
                {
                    ++Metrics.GeneticAbnormalities;
                }
                return true;
            }
        }
        return false;
    }
    
    private async void Work(object state)
    {
        long tick = 0;
        while (Active)
        {
            await Task.Delay(250);

            unsafe //rollover to 0 if max long value hit
            {
                ++tick;
            }
            
            List<int> deceasedIndices = new List<int>();

            int cycleMinAge = Metrics.MaxAge;
            int n = Persons.Count;
            long liveMale = 0;
            long liveDonor = 0;
            long liveFemale = 0;
            while (n > 0)
            {
                
                IPerson current = Persons[--n];
                if (n > 1)
                {
                    bool succeeded = false;
                    for (int i = 0; i < 3 && !succeeded; i++)
                    {
                        succeeded = AttemptMate(current, n, tick);
                    }
                }

                //TODO: Age from start tick of indiv
                bool aged = (tick - current.BirthTick) % 10 == 0;
                int curAge = aged ? ((BasePerson)current).Age++ : current.Age;
                
                if (curAge > Metrics.MaxAge) Metrics.MaxAge = curAge;
                if (curAge < cycleMinAge)
                {
                    cycleMinAge = curAge;
                }
                
                bool death = !aged && Roller.RollDeath(curAge);
                if (death)
                {
                    deceasedIndices.Add(n);
                }
                else
                {
                    if (current.Gender == 'M')
                    {
                        ++liveMale;
                        if (current is IDonor)
                        {
                            ++liveDonor;
                        }
                    }
                    else
                    {
                        ++liveFemale;
                    }
                }
            }
            
            Metrics.MinAge = cycleMinAge;
            Metrics.LiveMalePersons = liveMale;
            Metrics.LiveFemalePersons = liveFemale;
                
            foreach (int removal in deceasedIndices)
            {
                Persons.RemoveAt(removal);
                Metrics.DeceasedPersons++;
                Metrics.LivePersons--;
            }

            if (Persons.Count == 0)
            {
                Active = false; //terminate simulation
            }
        }
    }

    private bool AttemptMate(IPerson current, int n, long tick)
    {
        IPerson other = Persons[Random.Next(0, n)];
        if (CanMate(current, other))
        {
            if (current is MalePerson mp)
            {
                return Mate(mp, (FemalePerson)other, tick);
            }
            else if (current is FemalePerson fp)
            {
                return Mate((MalePerson)other, fp, tick);
            }
        }
        return false;
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