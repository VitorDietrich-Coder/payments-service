using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payments.Microservice.Domain.Enums
{
    public enum PaymentStatus
    {
        Pending = 0,
        Completed = 1,
        Failed = 2
    }
}
