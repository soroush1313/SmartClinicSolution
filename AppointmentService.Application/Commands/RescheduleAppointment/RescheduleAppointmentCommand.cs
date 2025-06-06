using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppointmentService.Application.Commands.RescheduleAppointment
{
    public class RescheduleAppointmentCommand
    {
        public Guid AppointmentId { get; set; }
        public DateTime NewStartTime { get; set; }
        public DateTime NewEndTime { get; set; }
    }
}
