using Bogus;
using Bogus.DataSets;

namespace WASM.Utils;

public static class NameGenerator
{
    private static HashSet<string> MaleFirstNames = new ();
    private static HashSet<string> MaleLastNames = new();
    private static HashSet<string> FemaleFirstNames = new ();
    private static HashSet<string> FemaleLastNames = new ();
    private static Random Random = new ();
    static NameGenerator()
    {
        Faker faker = new Faker();
        for (int i = 0; i < 1000; i++)
        {
            string mFirstName = faker.Name.FirstName(Name.Gender.Male);
            string mLastName = faker.Name.FirstName(Name.Gender.Male);
            string fFirstName = faker.Name.FirstName(Name.Gender.Female);
            string fLastName = faker.Name.FirstName(Name.Gender.Female);

            MaleFirstNames.Add(mFirstName);
            MaleLastNames.Add(mLastName);
            FemaleFirstNames.Add(fFirstName);
            FemaleLastNames.Add(fLastName);
        }
    }

    public static (string FirstName, string LastName) GetName(Name.Gender gender)
    {
        if (gender == Name.Gender.Male)
        {
            return (MaleFirstNames.ElementAt(Random.Next(0, MaleFirstNames.Count))
                , MaleLastNames.ElementAt(Random.Next(0, MaleLastNames.Count)));
        }
        return (FemaleFirstNames.ElementAt(Random.Next(0, FemaleFirstNames.Count))
            , FemaleLastNames.ElementAt(Random.Next(0, FemaleLastNames.Count)));
    }
    
    public static (Name.Gender gender, string FirstName, string LastName) GetName()
    {
        var gender = Random.Next(0, 2) == 1 ? Name.Gender.Male : Name.Gender.Female;
        if (gender == Name.Gender.Male)
        {
            return (gender, MaleFirstNames.ElementAt(Random.Next(0, MaleFirstNames.Count))
                , MaleLastNames.ElementAt(Random.Next(0, MaleLastNames.Count)));
        }
        return (gender, FemaleFirstNames.ElementAt(Random.Next(0, FemaleFirstNames.Count))
            , FemaleLastNames.ElementAt(Random.Next(0, FemaleLastNames.Count)));
    }
    
    
}