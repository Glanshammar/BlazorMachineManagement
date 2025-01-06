using BlazorMachineManagement.Data;
using Microsoft.EntityFrameworkCore;
using BlazorMachineManagement.Models;

namespace BlazorMachineManagement.Services;

public class MachineService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<MachineService> _logger;

    public MachineService(ApplicationDbContext context, ILogger<MachineService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<bool> StartMachineAsync(Guid id)
    {
        var machine = await _context.Machine.FindAsync(id);
        if (machine != null && !machine.IsOnline)
        {
            machine.IsOnline = true;
            machine.LastDataSent = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            _logger.LogInformation("Machine {MachineId} started successfully", id);
            return true;
        }
        _logger.LogWarning("Failed to start machine {MachineId}. Machine not found or already online", id);
        return false;
    }

    public async Task<bool> StopMachineAsync(Guid id)
    {
        var machine = await _context.Machine.FindAsync(id);
        if (machine != null && machine.IsOnline)
        {
            machine.IsOnline = false;
            machine.LastDataSent = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            _logger.LogInformation("Machine {MachineId} stopped successfully", id);
            return true;
        }
        _logger.LogWarning("Failed to stop machine {MachineId}. Machine not found or already offline", id);
        return false;
    }

    public async Task<bool> UpdateMachineDataAsync(Guid id, double temperature, int productionRate)
    {
        var machine = await _context.Machine.FindAsync(id);
        if (machine != null)
        {
            machine.LastDataSent = DateTime.UtcNow;
            machine.Temperature = temperature;
            machine.ProductionRate = productionRate;

            if (temperature > machine.MaxTemperature)
            {
                await StopMachineAsync(id);
                _logger.LogWarning("Machine {MachineId} stopped due to overheating. Temperature: {Temperature}, Max: {MaxTemperature}", 
                    id, temperature, machine.MaxTemperature);
            }

            if (productionRate == 0 && machine.IsOnline)
            {
                _logger.LogWarning("Machine {MachineId} is online but not producing", id);
            }

            await _context.SaveChangesAsync();
            _logger.LogInformation("Machine {MachineId} data updated. Temperature: {Temperature}, Production Rate: {ProductionRate}", 
                id, temperature, productionRate);
            return true;
        }
        _logger.LogWarning("Failed to update data for machine {MachineId}. Machine not found", id);
        return false;
    }

    public async Task<bool> SetMaxTemperatureAsync(Guid id, double maxTemperature)
    {
        var machine = await _context.Machine.FindAsync(id);
        if (machine != null)
        {
            machine.MaxTemperature = maxTemperature;
            await _context.SaveChangesAsync();
            _logger.LogInformation("Max temperature for machine {MachineId} set to {MaxTemperature}", id, maxTemperature);
            return true;
        }
        _logger.LogWarning("Failed to set max temperature for machine {MachineId}. Machine not found", id);
        return false;
    }

    public async Task<Machine?> GetMachineStatusAsync(Guid id)
    {
        var machine = await _context.Machine
            .Select(m => new Machine
            {
                Id = m.Id,
                Name = m.Name,
                IsOnline = m.IsOnline,
                LastDataSent = m.LastDataSent,
                Temperature = m.Temperature,
                ProductionRate = m.ProductionRate,
                MaxTemperature = m.MaxTemperature
            })
            .FirstOrDefaultAsync(m => m.Id == id);

        if (machine == null)
        {
            _logger.LogWarning("Machine {MachineId} not found when retrieving status", id);
        }
        else
        {
            _logger.LogInformation("Retrieved status for machine {MachineId}", id);
        }

        return machine;
    }
}