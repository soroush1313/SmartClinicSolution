using AppointmentService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppointmentService.Application.Interfaces
{
    public interface IAppointmentRepository
    {
        Task AddAsync(Appointment appointment);

        Task<bool> IsTimeSlotAvailable(Guid doctorId, DateTime startTime, DateTime endTime);

        Task<List<Appointment>> GetAppointmentsByDoctorAndDate(Guid doctorId, DateTime date);

        // 🔹 جدید برای Cancel و Reschedule
        Task<Appointment?> GetByIdAsync(Guid id);

        Task DeleteAsync(Appointment appointment);

        Task UpdateAsync(Appointment appointment);
        Task<List<Appointment>> GetAppointmentsByDoctorIdAsync(Guid doctorId);
        Task<List<Appointment>> GetAppointmentsByPatientIdAsync(Guid patientId);


    }
}
