using BlazorMachineManagement.Data;
using BlazorMachineManagement.DTO;
using Microsoft.EntityFrameworkCore;
using BlazorMachineManagement.Models;
using RestSharp;

namespace BlazorMachineManagement.Services;

public class MachineService
{
    private readonly RestClient _client;
    private readonly ILogger<MachineService> _logger;

    public MachineService(IConfiguration configuration, ILogger<MachineService> logger)
    {
        _client = new RestClient(configuration["API:BaseUrl"]);
        _logger = logger;
    }

    public async Task<List<Machine>> GetMachinesAsync()
    {
        var request = new RestRequest("api/Machines", Method.Get);
        var response = await _client.ExecuteAsync<List<Machine>>(request);
        
        if (response.IsSuccessful)
            return response.Data ?? new List<Machine>();
        
        _logger.LogError("Error retrieving machines: {ErrorMessage}", response.ErrorMessage);
        throw new InvalidOperationException($"Failed to retrieve machines.");
    }

    public async Task<bool> CreateMachineAsync(MachineDataDto machineData)
    {
        var request = new RestRequest("api/Machines", Method.Post);
        request.AddJsonBody(machineData);
        var response = await _client.ExecuteAsync(request);
        
        if (response.IsSuccessful)
            return true;
        
        _logger.LogError("Error creating machine: {ErrorMessage}", response.ErrorMessage);
        throw new InvalidOperationException($"Failed to create machine.");
    }

    public async Task<bool> UpdateMachineAsync(Guid id, MachineDataDto machineData)
    {
        var request = new RestRequest($"api/Machines/{id}/updateData", Method.Patch);
        request.AddJsonBody(machineData);
        var response = await _client.ExecuteAsync(request);
        
        if (response.IsSuccessful)
            return true;
        
        _logger.LogError("Error updating machine {Id}: {ErrorMessage}", id, response.ErrorMessage);
        throw new InvalidOperationException($"Failed to update machine {id}.");
    }

    public async Task<bool> DeleteMachineAsync(Guid id)
    {
        var request = new RestRequest($"api/Machines/{id}", Method.Delete);
        var response = await _client.ExecuteAsync(request);
        
        if (response.IsSuccessful)
            return true;
        
        _logger.LogError("Error deleting machine {Id}: {ErrorMessage}", id, response.ErrorMessage);
        throw new InvalidOperationException($"Failed to delete machine {id}.");
    }

    public async Task<bool> StartMachineAsync(Guid id)
    {
        var request = new RestRequest($"api/Machines/{id}/start", Method.Patch);
        var response = await _client.ExecuteAsync(request);
        
        if (response.IsSuccessful)
            return true;
        
        _logger.LogError("Error starting machine {Id}: {ErrorMessage}", id, response.ErrorMessage);
        throw new InvalidOperationException($"Failed to start machine {id}.");
    }

    public async Task<bool> StopMachineAsync(Guid id)
    {
        var request = new RestRequest($"api/Machines/{id}/stop", Method.Patch);
        var response = await _client.ExecuteAsync(request);
        
        if (response.IsSuccessful)
            return true;
        
        _logger.LogError("Error stopping machine {Id}: {ErrorMessage}", id, response.ErrorMessage);
        throw new InvalidOperationException($"Failed to stop machine {id}.");
    }
}