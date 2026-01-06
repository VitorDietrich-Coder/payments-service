using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payments.Microservice.Domain.Interfaces
{
  
    public interface IEventBus
    {
        Task PublishAsync<TEvent>(TEvent @event, string queueName)
            where TEvent : class;
    }

}
