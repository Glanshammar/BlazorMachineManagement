using System.ComponentModel.DataAnnotations;

namespace BlazorMachineManagement.Models;

public class Machine
{
    public Guid Id { get; set; }
    
    [Required]
    [StringLength(100, MinimumLength = 3)]
    public string Name { get; set; }
    
    [Required]
    public bool IsOnline { get; set; }
    
    public DateTime LastDataSent { get; set; }
    
    [Range(-273.15, 1000)]
    public double Temperature { get; set; }
    
    [Range(0, int.MaxValue)]
    public int ProductionRate { get; set; }
    
    [Required]
    [Range(0, 1000)]
    public double MaxTemperature { get; set; }
}