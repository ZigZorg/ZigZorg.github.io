namespace WASM.Utils;

public static class Roller
{
    private static Random Random = new Random();

    public static bool Roll(float percentChance)
    {
        int val = Random.Next(1, 101);
        return val <= percentChance;
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