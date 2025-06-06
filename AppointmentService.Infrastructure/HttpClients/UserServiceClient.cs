using AppointmentService.Application.DTOs;
using AppointmentService.Application.Interfaces;
using System.Net.Http.Json;

public class UserServiceClient : IUserServiceClient
{
    private readonly HttpClient _httpClient;

    public UserServiceClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<UserDto?> GetDoctorByIdAsync(Guid doctorId)
    {
        var response = await _httpClient.GetAsync($"api/UserInfo/doctor/{doctorId}");
        if (!response.IsSuccessStatusCode)
            return null;

        return await response.Content.ReadFromJsonAsync<UserDto>();
    }

    public async Task<UserDto?> GetPatientByIdAsync(Guid patientId)
    {
        var response = await _httpClient.GetAsync($"api/UserInfo/patient/{patientId}");
        if (!response.IsSuccessStatusCode)
            return null;

        return await response.Content.ReadFromJsonAsync<UserDto>();
    }
}
