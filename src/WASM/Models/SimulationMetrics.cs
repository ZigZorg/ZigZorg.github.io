namespace WASM.Models;

public class SimulationMetrics
{
    public int InitDonors { get; set; }
    public int InitMale { get; set; }
    public int InitFemale { get; set; }
    public double AverageAge { get; set; }
    public int MaxAge { get; set; }
    public int MinAge { get; set; }
    
    public long Births { get; set; }
    public long LivePersons { get; set; }
    public long DeceasedPersons { get; set; }
    public long PersonsAllTime { get; set; }
    public long GeneticAbnormalities { get; set; }
    public long Inbred { get; set; }
    public long LiveMalePersons { get; set; }
    public long LiveFemalePersons { get; set; }
    public long LiveDonors { get; set; }
}