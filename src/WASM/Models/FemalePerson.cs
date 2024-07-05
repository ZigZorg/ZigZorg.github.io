
using Bogus;
using WASM.Models;
using WASM.Utils;

public class FemalePerson : BasePerson
{
    public override char Gender => 'F';

    public IPerson? Mate(MalePerson other)
    {
        if (Age < 17)
        {
            return null;
        }
        if (other.Gender == 'F')
        {
            return null;
        }
        //https://www.nytimes.com/2011/09/06/health/06donor.html
        if (Roller.Roll(20))
        {
            bool boy = Roller.Roll(51);  //https://utswmed.org/medblog/it-boy-or-girl-fathers-family-might-provide-clue/#:~:text=But%20that's%20not%20exactly%20true,result%20in%20a%20baby%20boy.
            var gender = boy ? Bogus.DataSets.Name.Gender.Male : Bogus.DataSets.Name.Gender.Female;
            
            Faker<FemalePerson> faker = new Faker<FemalePerson>();
            faker.CustomInstantiator(f => new FemalePerson
            {
                Age = 0,
                FirstName = f.Name.FirstName(gender),
                LastName = f.Name.LastName(gender),
                Mother = this,
                Father = other
            });
            
            
            BasePerson ret = faker.Generate();
            
            ret.Inbred = (other.Mother == Mother || other.Father == Father)
                && (RootParents == false || other.RootParents == false);

            ret.Abnormality = Roller.Roll(ret.Inbred ? 4 : 2);
            
            
            return ret;
        }

        return null;
    }
}