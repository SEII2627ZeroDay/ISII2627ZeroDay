using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace AppForSEII.API.Logging; //change this instead of todoApi we put out thing sikosiko

public class RabbitMQLogger : ILogger, IDisposable
{
    private readonly string _name;
    private readonly RabbitMQLoggerConfiguration _config;
    private IConnection _connection;
    private IChannel _channel;
    private BasicProperties _properties;


  
    

    public RabbitMQLogger(string name, RabbitMQLoggerConfiguration config)
    {
        _name = name ?? throw new ArgumentNullException(nameof(name));
        _config = config ?? throw new ArgumentNullException(nameof(config));
        
        ValidateConfiguration(_config); 

        _properties = new BasicProperties
        {
            Persistent = _config.Durable,
            ContentType = "application/json"
        };

    }

    private static void ValidateConfiguration(RabbitMQLoggerConfiguration config)
    {
        if (string.IsNullOrEmpty(config.HostName))
            throw new ArgumentException("RabbitMQ HostName is required", nameof(config));
        if (config.Port <= 0)
            throw new ArgumentException("RabbitMQ Port must be greater than 0", nameof(config));
        if (string.IsNullOrEmpty(config.UserName))
            throw new ArgumentException("RabbitMQ UserName is required", nameof(config));
        if (string.IsNullOrEmpty(config.Password))
            throw new ArgumentException("RabbitMQ Password is required", nameof(config));
        if (string.IsNullOrEmpty(config.Exchange))
            throw new ArgumentException("RabbitMQ Exchange is required", nameof(config));
        if (string.IsNullOrEmpty(config.ExchangeType))
            throw new ArgumentException("RabbitMQ ExchangeType is required", nameof(config));
    }

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull => default;

    public bool IsEnabled(LogLevel logLevel) => logLevel != LogLevel.None;

    public void Log<TState>(
        LogLevel logLevel,
        EventId eventId,
        TState state,
        Exception? exception,
        Func<TState, Exception?, string> formatter)
    {
        if (!IsEnabled(logLevel))
            return;

        try
        {
            var logEntry = new
            {
                Timestamp = DateTime.UtcNow,
                LogLevel = logLevel.ToString(),
                Category = _name,
                EventId = eventId.Id,
                EventName = eventId.Name,
                Message = formatter(state, exception),
                Exception = exception?.ToString()
            };

        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error publishing log message to RabbitMQ: {ex.Message}");
        }

        var logJson = JsonConvert.SerializeObject(logJson); 
        var body = Encoding.UTF8.GetBytes(logJson); 
    }

      public async Task InitializeAsync()
    {

        var factory = new ConnectionFactory 
        { 
            HostName = _config.HostName, 
            Port = _config.Port, 
            UserName = _config.UserName, 
            Password = _config.Password 
        };

        _connection = await factory.CreateConnectionAsync(); 
        _channel = await _connection.CreateChannelAsync(); 

        await _channel.ExchangeDeclareAsync( exchange: _config.Exchange, type: _config.ExchangeType, durable: _config.Durable ); 
        
    }

    public void Dispose()
    {
        try
        {
            _channel?.CloseAsync().GetAwaiter().GetResult();
            _channel?.Dispose();
            _connection?.CloseAsync().GetAwaiter().GetResult();
            _connection?.Dispose();
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error disposing RabbitMQ logger: {ex.Message}");
        }
        GC.SuppressFinalize(this);
    }
}