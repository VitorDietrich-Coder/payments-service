using MediatR;
using Payments.Microservice.Application.DTO;

namespace Payments.Microservice.Application.Queries;

public sealed record GetPaymentsQuery(
    Guid UserId
) : IRequest<IEnumerable<PaymentDto>>;
