namespace WASM.Utils;

public static class Roller
{
    private static Random Random = new Random();
    private static double TotalWeight;

    static Roller()
    {
        TotalWeight = 0;
        foreach (var entry in DeathRate)
        {
            TotalWeight += entry.Value;
        }
    }
    
    public static bool Roll(float percentChance)
    {
        int val = Random.Next(1, 101);
        return val <= percentChance;
    }

    private static Dictionary<Range, double> DeathRate = new Dictionary<Range, double>
    {
        { new Range(0, 4), 22.7 }
        , { new Range(5, 14), 13.7 }
        , { new Range(15, 24), 84.2 }
        , { new Range(25, 34), 159.5 }
        , { new Range(35, 44), 248.0 }
        , { new Range(45, 54), 473.5 }
        , { new Range(55, 64), 1038.9 }
        , { new Range(65, 74), 2072.3 }
        , { new Range(75, 84), 4997.0 }
        , { new Range(85, 10000), 15210.9 }
    };
    
    public static bool RollDeath(int age)
    {
        var key = DeathRate.Keys.FirstOrDefault(x => age >= x.Start.Value && age <= x.End.Value);
        double rate = DeathRate[key];
    
        return Random.NextDouble() * 100000 < rate;
    }

    public static void Shuffle<T>(this IList<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = Random.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
}