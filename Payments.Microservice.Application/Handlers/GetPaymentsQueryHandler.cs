using MediatR;
using Payments.Microservice.Application.DTO;
using Payments.Microservice.Application.Queries;
using Payments.Microservice.Domain.Interfaces;

namespace Payments.Microservice.Application.Handlers;

public sealed class GetPaymentsQueryHandler
    : IRequestHandler<GetPaymentsQuery, IEnumerable<PaymentDto>>
{
    private readonly IPaymentRepository _repository;

    public GetPaymentsQueryHandler(IPaymentRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<PaymentDto>> Handle(
        GetPaymentsQuery request,
        CancellationToken cancellationToken)
    {
        var payments = await _repository
            .GetByUserIdAsync(request.UserId, cancellationToken);

        return payments.Select(p => new PaymentDto
        {
            PaymentId = p.Id,
            GameId = p.GameId,
            Amount = p.Price.Value,
            Status = p.Status.ToString(),
         });
    }
}
