using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BlazorMachineManagement.Data;
using BlazorMachineManagement.Models;
using BlazorMachineManagement.DTO;
using Microsoft.Extensions.Logging;

namespace BlazorMachineManagement.Controllers;

[Route("api/[controller]")]
[ApiController]
public class MachinesController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<MachinesController> _logger;

    public MachinesController(ApplicationDbContext context, ILogger<MachinesController> logger)
    {
        _context = context;
        _logger = logger;
    }

    // GET: api/Machines
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Machine>>> GetMachine()
    {
        return await _context.Machine.ToListAsync();
    }

    // GET: api/Machines/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Machine>> GetMachine(Guid id)
    {
        var machine = await _context.Machine.FindAsync(id);

        if (machine == null)
        {
            return NotFound();
        }

        return machine;
    }
    
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Machine>>> GetMachines()
    {
        return await _context.Machine
            .Select(m => new Machine
            {
                Id = m.Id,
                Name = m.Name,
                IsOnline = m.IsOnline,
                LastDataSent = m.LastDataSent
            })
            .ToListAsync();
    }
    
    [HttpPatch("{id}/start")]
    public async Task<IActionResult> StartMachine(Guid id)
    {
        var machine = await _context.Machine.FindAsync(id);
        if (machine == null)
        {
            return NotFound();
        }

        machine.IsOnline = true;
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpPatch("{id}/stop")]
    public async Task<IActionResult> StopMachine(Guid id)
    {
        var machine = await _context.Machine.FindAsync(id);
        if (machine == null)
        {
            return NotFound();
        }

        machine.IsOnline = false;
        await _context.SaveChangesAsync();

        return NoContent();
    }
    
    [HttpPatch("{id}/updateData")]
    public async Task<IActionResult> UpdateMachineData(Guid id, [FromBody] MachineDataDto dataUpdate)
    {
        try
        {
            var machine = await _context.Machine.FindAsync(id);
            if (machine == null)
            {
                _logger.LogWarning("Machine with ID {MachineId} not found for data update.", id);
                return NotFound($"Machine with ID {id} not found.");
            }

            // Update machine properties
            machine.LastDataSent = DateTime.UtcNow;
            machine.Temperature = dataUpdate.Temperature;
            machine.ProductionRate = dataUpdate.ProductionRate;
            machine.IsOnline = dataUpdate.IsOnline;
            machine.Name = dataUpdate.Name;
            
            // Add any custom logic here
            if (dataUpdate.Temperature > machine.MaxTemperature)
            {
                _logger.LogWarning("Machine {MachineId} temperature exceeds maximum: {Temperature}", id, dataUpdate.Temperature);
                machine.IsOnline = false;
            }

            await _context.SaveChangesAsync();

            _logger.LogInformation("Data updated for machine {MachineId}", id);
            return Ok(new { message = "Machine data updated successfully", machine = machine });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating data for machine {MachineId}", id);
            return StatusCode(500, "An error occurred while updating the machine data.");
        }
    }

    // PUT: api/Machines/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id}")]
    public async Task<IActionResult> PutMachine(Guid id, Machine machine)
    {
        if (id != machine.Id)
        {
            return BadRequest();
        }

        _context.Entry(machine).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!MachineExists(id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return NoContent();
    }

    // POST: api/Machines
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    public async Task<ActionResult<Machine>> PostMachine(Machine machine)
    {
        machine.Id = Guid.NewGuid();
        machine.IsOnline = false;
        machine.LastDataSent = DateTime.UtcNow;

        _context.Machine.Add(machine);
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetMachine", new { id = machine.Id }, machine);
    }

    // DELETE: api/Machines/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteMachine(Guid id)
    {
        var machine = await _context.Machine.FindAsync(id);
        if (machine == null)
        {
            return NotFound();
        }

        _context.Machine.Remove(machine);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool MachineExists(Guid id)
    {
        return _context.Machine.Any(e => e.Id == id);
    }
}