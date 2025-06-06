using AppointmentService.Application.Interfaces.Messaging;
using RabbitMQ.Client;
using System.Text.Json;
using System.Text;

public class RabbitMqPublisher : IMessagePublisher
{
    private readonly IModel _channel;
    private readonly IConnection _connection;

    public RabbitMqPublisher(string hostName = "localhost")
    {
        var factory = new ConnectionFactory() { HostName = hostName };
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();

        _channel.QueueDeclare(queue: "appointments",
                              durable: false,
                              exclusive: false,
                              autoDelete: false,
                              arguments: null);
    }

    public Task PublishAsync<T>(T message)
    {
        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));

        _channel.BasicPublish(exchange: "",
                              routingKey: "appointments",
                              basicProperties: null,
                              body: body);

        return Task.CompletedTask;
    }
}
