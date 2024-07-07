namespace WASM.Models;

public abstract class BasePerson : IPerson
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Name => $"{FirstName} {LastName}";
    public abstract char Gender { get; }
    public int Age { get; set;  }
    public IPerson? Mother { get; set; }
    public IPerson? Father { get; set; }
    public bool Inbred { get; set; }
    public int InbredDegree { get; set; } //TODO
    public bool Abnormality { get; set; }
    public int MateCount { get; set; }
    
    public bool RootParents => Mother is RootParent && Father is RootParent;

}