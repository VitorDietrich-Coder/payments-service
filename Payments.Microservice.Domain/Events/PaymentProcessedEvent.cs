using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payments.Microservice.Domain.Events
{
    public record PaymentProcessedEvent(Guid PaymentId, Guid UserId, decimal Amount, string Status);

}
