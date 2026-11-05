using RabbitMQ.AMQP.Client;
using RabbitMQ.AMQP.Client.Impl;

namespace UserService.Services;

public class RabbitMQConnection : IAsyncDisposable
{
    private readonly IEnvironment _environment;
    private readonly Lazy<Task<IConnection>> _connection;

    public RabbitMQConnection(IConfiguration configuration)
    {
        string uri = configuration["RabbitMQ:Uri"]!;
        string containerId = configuration["RabbitMQ:ContainerId"]!;

        ConnectionSettings settings = ConnectionSettingsBuilder.Create()
            .Uri(new Uri(uri))
            .ContainerId(containerId)
            .Build();

        _environment = AmqpEnvironment.Create(settings);

        _connection = new Lazy<Task<IConnection>>(
            () => _environment.CreateConnectionAsync());
    }

    public Task<IConnection> GetConnectionAsync()
    {
        return _connection.Value;
    }

    public async ValueTask DisposeAsync()
    {
        if (_connection.IsValueCreated && _connection.Value.IsCompletedSuccessfully)
        {
            await _connection.Value.Result.CloseAsync();
        }

        await _environment.CloseAsync();
    }
}