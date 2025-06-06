using AppointmentService.Application.Interfaces.Messaging;
using AppointmentService.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppointmentService.Application.Commands.CancelAppointment
{
    public class CancelAppointmentHandler
    {
        private readonly IAppointmentRepository _repository;
        private readonly ICacheService _cache;
        private readonly IMessagePublisher _publisher;

        public CancelAppointmentHandler(IAppointmentRepository repository, ICacheService cache, IMessagePublisher publisher)
        {
            _repository = repository;
            _cache = cache;
            _publisher = publisher;
        }

        public async Task<bool> Handle(CancelAppointmentCommand command)
        {
            var appointment = await _repository.GetByIdAsync(command.AppointmentId);
            if (appointment == null)
                throw new Exception("Appointment not found");

            await _repository.DeleteAsync(appointment);
            await _cache.RemoveAsync($"doctor:{appointment.DoctorId}:slots");

            await _publisher.PublishAsync(new
            {
                Event = "AppointmentCancelled",
                AppointmentId = appointment.Id,
                DoctorId = appointment.DoctorId,
                PatientId = appointment.PatientId,
                StartTime = appointment.StartTime
            });

            return true;
        }

    }
}
