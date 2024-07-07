namespace WASM.Models;

public class RootParent : IPerson
{
    public string Name => "Root";
    public char Gender => '\0';
    public int Age => -1;
    public IPerson? Mother { get; } = null;
    public IPerson? Father { get; } = null;
    public int MateCount { get; } = -1;
}