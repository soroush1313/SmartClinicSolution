using AppointmentService.Application.DTOs;

public interface IUserServiceClient
{
    Task<UserDto?> GetDoctorByIdAsync(Guid doctorId);
    Task<UserDto?> GetPatientByIdAsync(Guid patientId);
}
