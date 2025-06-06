using AppointmentService.Application.Interfaces;
using AppointmentService.Application.Interfaces.Messaging;
using AppointmentService.Application.Messages;
using AppointmentService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppointmentService.Application.Commands.CreateAppointment
{
    public class CreateAppointmentHandler
    {
        private readonly IAppointmentRepository _repository;
        private readonly ICacheService _cache;
        private readonly IMessagePublisher _publisher;
        private readonly IUserServiceClient _userServiceClient;
        private readonly ILogService _logService;


        public CreateAppointmentHandler(IAppointmentRepository repository, ICacheService cache, IMessagePublisher publisher, IUserServiceClient userServiceClient, ILogService logService)
        {
            _repository = repository;
            _cache = cache;
            _publisher = publisher;
            _userServiceClient = userServiceClient;
            _logService = logService;
        }

        public async Task<Guid> Handle(CreateAppointmentCommand command)
        {
            var appointment = new Appointment
            {
                Id = Guid.NewGuid(),
                DoctorId = command.DoctorId,
                PatientId = command.PatientId,
                StartTime = command.StartTime,
                EndTime = command.EndTime,
                Status = "Scheduled"
            };

            await _repository.AddAsync(appointment);
            await _logService.LogAsync(new LogEntry
            {
                Id = Guid.NewGuid(),
                Source = "AppointmentService",
                Message = $"Appointment created for doctor {command.DoctorId}",
                Timestamp = DateTime.UtcNow
            });


            var doctor = await _userServiceClient.GetDoctorByIdAsync(command.DoctorId);
            var patient = await _userServiceClient.GetPatientByIdAsync(command.PatientId);

            await _cache.RemoveAsync($"doctor:{command.DoctorId}:slots");
            if (doctor == null || patient == null)
            {
                throw new InvalidOperationException("Doctor or patient not found.");
            }
            await _publisher.PublishAsync(new AppointmentCreatedEvent
            {
                AppointmentId = appointment.Id,
                DoctorId = appointment.DoctorId,
                PatientId = appointment.PatientId,
                StartTime = appointment.StartTime,
                EndTime = appointment.EndTime,
                DoctorName = doctor.FullName,
                DoctorEmail = doctor.Email,
                PatientName = patient.FullName,
                PatientEmail = patient.Email
            });

            return appointment.Id;
        }
    }
}
