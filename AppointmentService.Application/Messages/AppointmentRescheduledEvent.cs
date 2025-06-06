using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppointmentService.Application.Messages
{
    public class AppointmentRescheduledEvent
    {
        public Guid AppointmentId { get; set; }
        public DateTime NewStartTime { get; set; }
        public DateTime NewEndTime { get; set; }
    }
}
