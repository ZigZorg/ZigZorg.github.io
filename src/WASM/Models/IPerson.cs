public interface IPerson
{
    public string Name { get; }
    public char Gender { get; }
    public int Age { get; }
    public IPerson? Mother { get; }
    public IPerson? Father { get; }
}