 
using Microsoft.Extensions.Configuration;
using Payments.Microservice.Domain.Interfaces;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace Payments.Microservice.Infrastructure.Messaging;

public class RabbitMqEventBus : IDisposable, IEventBus
{
    private readonly IConnection _connection;
    private readonly IModel _channel;

    public RabbitMqEventBus(IConfiguration configuration)
    {
        var connectionString = configuration["RabbitMq:ConnectionString"];

        if (string.IsNullOrWhiteSpace(connectionString))
            throw new InvalidOperationException(
                "RabbitMq:ConnectionString is not configured.");

        var factory = new ConnectionFactory
        {
            Uri = new Uri(connectionString),
            DispatchConsumersAsync = true
        };

        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
    }
    /// <summary>
    /// Publica um evento na fila
    /// </summary>
    public Task PublishAsync<TEvent>(TEvent @event, string queueName) where TEvent : class
    {
        // Declara a fila (idempotente)
        _channel.QueueDeclare(
            queue: queueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null);

        // Serializa o evento
        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(@event));

        // Cria propriedades e define persistência
        var properties = _channel.CreateBasicProperties();
        properties.Persistent = true;

        // Publica na fila
        _channel.BasicPublish(
            exchange: "",
            routingKey: queueName,
            basicProperties: properties,
            body: body);

        return Task.CompletedTask;
    }

    /// <summary>
    /// Consome eventos da fila
    /// </summary>
    public void Subscribe<TEvent>(string queueName, Func<TEvent, Task> handler) where TEvent : class
    {
        _channel.QueueDeclare(
            queue: queueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null);

        var consumer = new RabbitMQ.Client.Events.EventingBasicConsumer(_channel);
        consumer.Received += async (sender, e) =>
        {
            var json = Encoding.UTF8.GetString(e.Body.ToArray());
            var @event = JsonSerializer.Deserialize<TEvent>(json);
            if (@event != null)
                await handler(@event);

            _channel.BasicAck(e.DeliveryTag, false);
        };

        _channel.BasicConsume(
            queue: queueName,
            autoAck: false,
            consumer: consumer);
    }

    public void Dispose()
    {
        _channel?.Close();
        _channel?.Dispose();
        _connection?.Close();
        _connection?.Dispose();
    }
}
