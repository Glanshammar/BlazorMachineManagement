using BlazorMachineManagement.Data;
using Microsoft.EntityFrameworkCore;
using BlazorMachineManagement.Models;

namespace BlazorMachineManagement.Services;

public class MachineService
    {
        private readonly ApplicationDbContext _context;

        public MachineService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Machine>> GetAllMachinesAsync()
        {
            return await _context.Machine.ToListAsync();
        }

        public async Task<Machine> GetMachineByIdAsync(Guid id)
        {
            return await _context.Machine.FindAsync(id);
        }

        public async Task<Machine> AddMachineAsync(Machine machine)
        {
            machine.Id = Guid.NewGuid();
            machine.IsOnline = false;
            machine.LastDataSent = DateTime.UtcNow;

            _context.Machine.Add(machine);
            await _context.SaveChangesAsync();

            return machine;
        }

        public async Task UpdateMachineAsync(Machine machine)
        {
            _context.Entry(machine).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteMachineAsync(Guid id)
        {
            var machine = await _context.Machine.FindAsync(id);
            if (machine != null)
            {
                _context.Machine.Remove(machine);
                await _context.SaveChangesAsync();
            }
        }

        public async Task StartMachineAsync(Guid id)
        {
            var machine = await _context.Machine.FindAsync(id);
            if (machine != null)
            {
                machine.IsOnline = true;
                await _context.SaveChangesAsync();
            }
        }

        public async Task StopMachineAsync(Guid id)
        {
            var machine = await _context.Machine.FindAsync(id);
            if (machine != null)
            {
                machine.IsOnline = false;
                await _context.SaveChangesAsync();
            }
        }

        public async Task UpdateMachineDataAsync(Guid id, string data)
        {
            var machine = await _context.Machine.FindAsync(id);
            if (machine != null)
            {
                machine.LastDataSent = DateTime.UtcNow;
                // Here you would typically update other properties based on the 'data' parameter
                // For example: machine.SomeProperty = ParseData(data);

                await _context.SaveChangesAsync();
            }
        }
    }