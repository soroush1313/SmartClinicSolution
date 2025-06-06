using AppointmentService.Application.Commands.RescheduleAppointment;
using AppointmentService.Application.Interfaces;
using AppointmentService.Application.Interfaces.Messaging;
using AppointmentService.Application.Messages;

public class RescheduleAppointmentHandler
{
    private readonly IAppointmentRepository _repository;
    private readonly ICacheService _cache;
    private readonly IMessagePublisher _publisher;

    public RescheduleAppointmentHandler(IAppointmentRepository repository, ICacheService cache, IMessagePublisher publisher)
    {
        _repository = repository;
        _cache = cache;
        _publisher = publisher;
    }

    public async Task<bool> Handle(RescheduleAppointmentCommand command)
    {
        var appointment = await _repository.GetByIdAsync(command.AppointmentId);
        if (appointment == null) throw new Exception("Appointment not found.");

        var isAvailable = await _repository.IsTimeSlotAvailable(appointment.DoctorId, command.NewStartTime, command.NewEndTime);
        if (!isAvailable) throw new Exception("The new time slot is not available.");

        appointment.StartTime = command.NewStartTime;
        appointment.EndTime = command.NewEndTime;
        await _repository.UpdateAsync(appointment);

        await _cache.RemoveAsync($"doctor:{appointment.DoctorId}:slots");

        await _publisher.PublishAsync(new AppointmentRescheduledEvent
        {
            AppointmentId = appointment.Id,
            NewStartTime = appointment.StartTime,
            NewEndTime = appointment.EndTime
        });

        return true;
    }
}
