 
using Microsoft.Extensions.Logging;
using Payments.Contracts.Events;
using Payments.Microservice.Domain.Interfaces;
using Payments.Microservice.Domain.ValueObjects;
 
namespace Payments.Microservice.Application.Handlers
{
    public sealed class GamePurchasedEventHandler
    {
        private readonly IPaymentRepository _repository;
        private readonly ILogger<GamePurchasedEventHandler> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEventBus _eventBus;

        public GamePurchasedEventHandler(
            IPaymentRepository repository,
            ILogger<GamePurchasedEventHandler> logger,
            IUnitOfWork unitOfWork,
            IEventBus eventBus)
        {
            _repository = repository;
            _logger = logger;
            _unitOfWork = unitOfWork;
            _eventBus = eventBus;
        }

        public async Task HandleAsync(GamePurchasedIntegrationEvent @event, CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "Processing payment | GameId={GameId} | UserId={UserId} | CorrelationId={CorrelationId}",
                @event.GameId,
                @event.UserId,
                @event.CorrelationId);

            var payment = new Payment(
                @event.UserId,
                @event.GameId,
                new CurrencyAmount(@event.Price));

            await _repository.AddAsync(payment);
            await _unitOfWork.CommitAsync(cancellationToken);

            payment.Complete();

            await _repository.UpdateAsync(payment);

            var integrationEvent = new PaymentCompletedIntegrationEvent(
                payment.Id,
                payment.GameId,
                payment.UserId,
                payment.Price.Value,
                @event.CorrelationId
            );

            await _eventBus.PublishAsync(
                integrationEvent,
                queueName: "payments.payment.completed");
        }
    }
}
