using System.Text.Json;
using RabbitMQ.AMQP.Client;
using RabbitMQ.AMQP.Client.Impl;
using UserService.Services;

namespace UserService.RabbitMQ;

public class RabbitMQService
{
    private const string ExchangeName = "users.events";

    private readonly RabbitMQConnection _rabbitMQ;
    private IPublisher? _publisher;

    public RabbitMQService(RabbitMQConnection rabbitMQ)
    {
        _rabbitMQ = rabbitMQ;
    }

    public async Task InitializeAsync()
    {
        var connection = await _rabbitMQ.GetConnectionAsync();

        // Opret exchange
        IManagement management = connection.Management();

        IExchangeSpecification exchange = management
            .Exchange(ExchangeName)
            .Type(ExchangeType.FANOUT);

        await exchange.DeclareAsync();

        // Opret publisher
        _publisher = await connection
            .PublisherBuilder()
            .Exchange(ExchangeName)
            .BuildAsync();
    }

    public async Task PublishUserDeletedAsync(int userId)
    {
        if (_publisher == null)
            throw new InvalidOperationException("Publisher not initialized.");

        var messageData = new
        {
            EventType = "UserDeleted",
            UserId = userId,
            OccurredAt = DateTimeOffset.UtcNow,
        };

        string json = JsonSerializer.Serialize(
            messageData,
            new JsonSerializerOptions(JsonSerializerDefaults.Web));

        var message = new AmqpMessage(json);

        PublishResult result = await _publisher.PublishAsync(message);

        if (result.Outcome.State != OutcomeState.Accepted)
        {
            throw new Exception("Could not publish UserDeleted event.");
        }
    }
}