using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppointmentService.Application.Interfaces.Messaging
{
    public interface IMessagePublisher
    {
        Task PublishAsync<T>(T message);
    }
}
