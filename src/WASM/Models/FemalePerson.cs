
using Bogus;
using WASM.Models;
using WASM.Utils;

public class FemalePerson : BasePerson
{
    public override char Gender => 'F';

    //TODO: Refactor and make more polymorphic
    public IPerson? Mate(MalePerson other, long tick)
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

            var name = NameGenerator.GetName(gender);

            BasePerson ret = gender == Bogus.DataSets.Name.Gender.Male
                ? new MalePerson
                {
                    Mother = this,
                    Father = other,
                    Age = 0,
                    BirthTick = tick,
                    FirstName = name.FirstName,
                    LastName = name.LastName
                }
                : new FemalePerson
                {
                    Mother = this,
                    Father = other,
                    Age = 0,
                    BirthTick = tick,
                    FirstName = name.FirstName,
                    LastName = name.LastName
                };
            
            ret.Inbred = (other.Mother == Mother || other.Father == Father)
                && (RootParents == false || other.RootParents == false);

            ret.Abnormality = Roller.Roll(ret.Inbred ? 4 : 2); //2% for normal, 4% for inbred

            MateCount++;
            other.MateCount++;
            
            return ret;
        }

        return null;
    }
}