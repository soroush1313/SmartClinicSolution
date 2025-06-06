using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppointmentService.Application.Queries.GetAvailableSlots
{
    public class GetAvailableSlotsQuery
    {
        public Guid DoctorId { get; set; }
        public DateTime Date { get; set; } // فقط یک روز
    }
}
