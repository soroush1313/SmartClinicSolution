using AppointmentService.Application.Commands.CancelAppointment;
using AppointmentService.Application.Commands.CreateAppointment;
using AppointmentService.Application.Commands.RescheduleAppointment;
using AppointmentService.Application.Interfaces;
using AppointmentService.Application.Queries.GetAvailableSlots;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class AppointmentController : ControllerBase
{
    private readonly CreateAppointmentHandler _createHandler;
    private readonly GetAvailableSlotsHandler _slotsHandler;
    private readonly CancelAppointmentHandler _cancelHandler;

    public AppointmentController(
        CreateAppointmentHandler createHandler,
        GetAvailableSlotsHandler slotsHandler,
        CancelAppointmentHandler cancelHandler)
    {
        _createHandler = createHandler;
        _slotsHandler = slotsHandler;
        _cancelHandler = cancelHandler;
    }

    [HttpPost]
    public async Task<IActionResult> CreateAppointment([FromBody] CreateAppointmentCommand command)
    {
        try
        {
            var appointmentId = await _createHandler.Handle(command);
            return Ok(new { AppointmentId = appointmentId });
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("slots")]
    public async Task<IActionResult> GetAvailableSlots([FromQuery] Guid doctorId, [FromQuery] DateTime date)
    {
        var query = new GetAvailableSlotsQuery { DoctorId = doctorId, Date = date };
        var result = await _slotsHandler.Handle(query);
        return Ok(result);
    }

    [HttpDelete("{appointmentId}")]
    public async Task<IActionResult> CancelAppointment(Guid appointmentId)
    {
        var handler = HttpContext.RequestServices.GetRequiredService<CancelAppointmentHandler>();
        try
        {
            var result = await _cancelHandler.Handle(new CancelAppointmentCommand { AppointmentId = appointmentId });
            return Ok(new { Success = result });
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{appointmentId}/reschedule")]
    public async Task<IActionResult> RescheduleAppointment(Guid appointmentId, [FromBody] RescheduleAppointmentCommand command)
    {
        var handler = HttpContext.RequestServices.GetRequiredService<RescheduleAppointmentHandler>();

        if (appointmentId != command.AppointmentId)
            return BadRequest("Mismatched appointment ID");

        try
        {
            var result = await handler.Handle(command);
            return Ok(new { Success = result });
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("reschedule")]
    public async Task<IActionResult> RescheduleAppointment([FromBody] RescheduleAppointmentCommand command)
    {
        var handler = HttpContext.RequestServices.GetRequiredService<RescheduleAppointmentHandler>();

        try
        {
            var result = await handler.Handle(command);
            return Ok(new { Success = result });
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("doctor/{doctorId}")]
    public async Task<IActionResult> GetAppointmentsByDoctor(Guid doctorId)
    {
        var repo = HttpContext.RequestServices.GetRequiredService<IAppointmentRepository>();
        var appointments = await repo.GetAppointmentsByDoctorIdAsync(doctorId);
        return Ok(appointments);
    }

    [HttpGet("patient/{patientId}")]
    public async Task<IActionResult> GetAppointmentsByPatient(Guid patientId)
    {
        var repo = HttpContext.RequestServices.GetRequiredService<IAppointmentRepository>();
        var appointments = await repo.GetAppointmentsByPatientIdAsync(patientId);
        return Ok(appointments);
    }


}
