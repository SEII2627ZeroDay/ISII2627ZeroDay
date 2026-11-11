using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
namespace AppForSEII.LogViewer;
public class Subscriber : IAsyncDisposable
{
    private readonly IConfiguration _configuration;
    private readonly string _exchangeName;
    private IConnection? _connection;
    private IChannel? _channel;
    public Subscriber(IConfiguration configuration)
    {
        _configuration = configuration;
        _exchangeName = _configuration["RabbitMQ:Exchange"] ?? "logs";
    }
    public async Task InitializeConnectionAsync()
    {
        int port = int.TryParse(_configuration["RabbitMQ:Port"], out int p) ? p : 5672;
        var factory = new ConnectionFactory
        {
            HostName = _configuration["RabbitMQ:HostName"] ?? "localhost", Port = port,
            UserName = _configuration["RabbitMQ:UserName"] ?? "guest",
            Password = _configuration["RabbitMQ:Password"] ?? "guest"
        };
        // Steps 1 & 2: Open TCP connection and AMQP channel
        _connection = await factory.CreateConnectionAsync();
        _channel = await _connection.CreateChannelAsync();
        // Step 3: Declare the exact same fanout exchange as the API (autoDelete: capital D!)
        await _channel.ExchangeDeclareAsync(_exchangeName, ExchangeType.Fanout, durable: true, autoDelete: false);
    }
    public async Task StartConsumingAsync()
    {
        if (_channel is null) throw new InvalidOperationException("Call InitializeConnectionAsync first.");
        // Step 4 & 5: Create ephemeral queue, get generated name, and bind it to "logs"
        var tempQueue = await _channel.QueueDeclareAsync();
        var queueName = tempQueue.QueueName;
        await _channel.QueueBindAsync(queue: queueName, exchange: _exchangeName, routingKey: string.Empty);
        // Steps 6 & 7: Create async consumer and register ReceivedAsync callback
        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.ReceivedAsync += async (model, ea) =>
        {
            var json = Encoding.UTF8.GetString(ea.Body.ToArray());
            try
            {
                var log = JsonSerializer.Deserialize<LogEntry>(json);
                if (log is not null) PrintFormattedLog(log);
            }
            catch (JsonException) { Console.WriteLine($"[RAW] {json}"); }
            await Task.CompletedTask;
        };
        // Step 8: Begin receiving logs (autoAck: true because logs are read-only)
        await _channel.BasicConsumeAsync(queue: queueName, autoAck: true, consumer: consumer);
        Console.WriteLine($"[*] LogViewer listening on '{_exchangeName}' (temp queue: '{queueName}')...");
    }
    private static void PrintFormattedLog(LogEntry log)
    {
        var prev = Console.ForegroundColor;
        Console.ForegroundColor = log.LogLevel switch
        {
            "Critical" or "Error" => ConsoleColor.Red, "Warning" => ConsoleColor.Yellow,
            "Information" => ConsoleColor.Green, _ => ConsoleColor.Cyan
        };
        Console.WriteLine($"[{log.Timestamp:yyyy-MM-dd HH:mm:ss}] [{log.LogLevel.ToUpper()}] ({log.Category})");
        Console.ForegroundColor = prev;
        Console.WriteLine($"   Message: {log.Message}");
        if (!string.IsNullOrWhiteSpace(log.Exception)) Console.WriteLine($"   Exception: {log.Exception}");
        Console.WriteLine(new string('-', 60));
    }
    public async ValueTask DisposeAsync()
    {
        if (_channel is not null) { await _channel.CloseAsync(); await _channel.DisposeAsync(); }
        if (_connection is not null) { await _connection.CloseAsync(); await _connection.DisposeAsync(); }
    }
}