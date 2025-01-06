using System.ComponentModel.DataAnnotations;

namespace BlazorMachineManagement.DTO;

public class MachineDataDto
{
    [Required]
    [StringLength(50, MinimumLength = 3)]
    public string Name { get; set; }
    
    [Required]
    public bool IsOnline { get; set; }
    
    [Required]
    [Range(-273.15, 1000)]
    public double Temperature { get; set; }
    
    [Required]
    [Range(0, int.MaxValue)]
    public int ProductionRate { get; set; }
}