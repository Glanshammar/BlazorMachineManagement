using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BlazorMachineManagement.Data;
using BlazorMachineManagement.Models;

namespace BlazorMachineManagement.Controllers;

[Route("api/[controller]")]
[ApiController]
public class MachinesController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public MachinesController(ApplicationDbContext context)
    {
        _context = context;
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