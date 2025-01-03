namespace BlazorMachineManagement.Models;

public class Machine
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public bool IsOnline { get; set; }
    public DateTime LastDataSent { get; set; }
}