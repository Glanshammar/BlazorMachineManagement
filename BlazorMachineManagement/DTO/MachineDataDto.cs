namespace BlazorMachineManagement.DTO;

public class MachineDataDto
{
    public string Name { get; set; }
    public bool IsOnline { get; set; }
    public double Temperature { get; set; }
    public int ProductionRate { get; set; }
}