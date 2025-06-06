using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppointmentService.Application.DTOs
{
    public class TimeSlotDto
    {
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
    }
}
