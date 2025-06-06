using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppointmentService.Application.Messages
{
    public class AppointmentCreatedEvent
    {
        public Guid AppointmentId { get; set; }
        public Guid DoctorId { get; set; }
        public Guid PatientId { get; set; }
        public string DoctorName { get; set; } = null!;
        public string DoctorEmail { get; set; } = null!;
        public string PatientName { get; set; } = null!;
        public string PatientEmail { get; set; } = null!;
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }
}
