using Microsoft.AspNetCore.Mvc;
using UserService.DTOs;
using UserService.Persistence;

[ApiController]
[Route("api/[controller]")]
public class UserInfoController : ControllerBase
{
    private readonly AppDbContext _context;

    public UserInfoController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet("emails")]
    public async Task<IActionResult> GetEmails([FromQuery] Guid doctorId, [FromQuery] Guid patientId)
    {
        var doctor = await _context.Doctors.FindAsync(doctorId);
        var patient = await _context.Patients.FindAsync(patientId);

        if (doctor == null || patient == null)
            return NotFound("Doctor or patient not found");

        return Ok(new
        {
            DoctorName = doctor.FullName,
            DoctorEmail = doctor.Email,
            PatientName = patient.FullName,
            PatientEmail = patient.Email
        });
    }
    [HttpGet("doctor/{id}")]
    public async Task<IActionResult> GetDoctorById(Guid id)
    {
        var doctor = await _context.Doctors.FindAsync(id);
        if (doctor == null)
            return NotFound();

        return Ok(new UserDto
        {
            Id = doctor.Id,
            FullName = doctor.FullName,
            Email = doctor.Email
        });
    }

    [HttpGet("patient/{id}")]
    public async Task<IActionResult> GetPatientById(Guid id)
    {
        var patient = await _context.Patients.FindAsync(id);
        if (patient == null)
            return NotFound();

        return Ok(new UserDto
        {
            Id = patient.Id,
            FullName = patient.FullName,
            Email = patient.Email
        });
    }

}
