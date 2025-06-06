using Microsoft.AspNetCore.Connections;
using RabbitMQ.Client;
using RabbitMQ.Client.Events; // ???? ????
using System.Net.WebSockets;
using System.Text;
using System.Collections.Generic;

var builder = WebApplication.CreateBuilder(args);

// WebSocket and CORS setup
builder.Services.AddCors();

var app = builder.Build();

app.UseCors(policy =>
    policy.AllowAnyHeader()
          .AllowAnyMethod()
          .AllowAnyOrigin()
);

app.UseWebSockets();

// ? ??? ?? ??????? ????? ??
List<WebSocket> _sockets = new();

var factory = new ConnectionFactory() { HostName = "localhost" };
var connection = factory.CreateConnection();
var channel = connection.CreateModel();

channel.QueueDeclare(queue: "appointments",
                     durable: false,
                     exclusive: false,
                     autoDelete: false,
                     arguments: null);

var consumer = new EventingBasicConsumer(channel);
consumer.Received += async (model, ea) =>
{
    var body = ea.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);

    Console.WriteLine("?? Message received from RabbitMQ: " + message);

    var messageBuffer = Encoding.UTF8.GetBytes(message);
    var segment = new ArraySegment<byte>(messageBuffer);

    foreach (var socket in _sockets.ToList())
    {
        if (socket.State == WebSocketState.Open)
        {
            await socket.SendAsync(segment, WebSocketMessageType.Text, true, CancellationToken.None);
        }
    }
};

channel.BasicConsume(queue: "appointments", autoAck: true, consumer: consumer);

app.Map("/ws", async context =>
{
    if (context.WebSockets.IsWebSocketRequest)
    {
        var webSocket = await context.WebSockets.AcceptWebSocketAsync();
        _sockets.Add(webSocket);
        Console.WriteLine("? WebSocket connection established");

        var buffer = new byte[1024 * 4];
        while (webSocket.State == WebSocketState.Open)
        {
            var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            if (result.MessageType == WebSocketMessageType.Close)
            {
                _sockets.Remove(webSocket);
                await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closed", CancellationToken.None);
            }
        }
    }
    else
    {
        context.Response.StatusCode = 400;
    }
});

app.Run();
