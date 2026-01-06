using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Payments.Contracts.Events;
using Payments.Microservice.Application.Handlers;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

public sealed class GamePurchasedRabbitConsumer : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IConfiguration _configuration;
    private readonly ILogger<GamePurchasedRabbitConsumer> _logger;
    private IModel _channel;

    public GamePurchasedRabbitConsumer(
        IServiceScopeFactory scopeFactory,
        IConfiguration configuration,
        ILogger<GamePurchasedRabbitConsumer> logger)
    {
        _scopeFactory = scopeFactory;
        _configuration = configuration;
        _logger = logger;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var factory = new ConnectionFactory
        {
            Uri = new Uri(_configuration["RabbitMq:ConnectionString"]),
            DispatchConsumersAsync = true
        };

        var connection = factory.CreateConnection();
        _channel = connection.CreateModel();

        _channel.ExchangeDeclare(
            exchange: "games.events",
            type: ExchangeType.Topic,
            durable: true);

        _channel.QueueDeclare(
            queue: "payments.game.purchased",
            durable: true,
            exclusive: false,
            autoDelete: false);

        _channel.QueueBind(
            queue: "payments.game.purchased",
            exchange: "games.events",
            routingKey: "game.purchased");

        var consumer = new AsyncEventingBasicConsumer(_channel);

        consumer.Received += async (_, args) =>
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();

                var handler = scope.ServiceProvider
                    .GetRequiredService<GamePurchasedEventHandler>();

                var json = Encoding.UTF8.GetString(args.Body.ToArray());

                var @event = JsonSerializer.Deserialize<GamePurchasedIntegrationEvent>(
                    json,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                if (@event is null)
                    throw new InvalidOperationException("Evento inválido");

                await handler.HandleAsync(@event, default);

                _channel.BasicAck(args.DeliveryTag, false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao processar evento GamePurchased");

                _channel.BasicNack(args.DeliveryTag, false, requeue: false);
            }
        };

        _channel.BasicConsume(
            queue: "payments.game.purchased",
            autoAck: false,
            consumer: consumer);

        _logger.LogInformation("Payments consumer iniciado");

        stoppingToken.Register(() =>
        {
            _logger.LogInformation("Payments consumer stopping");

            _channel.Close();
            connection.Close();
        });

        return Task.CompletedTask;
    }

}
