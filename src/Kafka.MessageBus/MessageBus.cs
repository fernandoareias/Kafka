using System.Text.Json;
using Confluent.Kafka;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using Kafka.MessageBus.Interfaces;
using Kafka.MessageBus.Serializers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;


namespace Kafka.MessageBus;

public class MessageBus : IMessageBus
{
    private readonly ILogger<MessageBus> _logger;
    private readonly string _kafkaServer;
    private readonly string _groupId;
    
    public MessageBus(IConfiguration configuration, ILogger<MessageBus> logger)
    {
        _kafkaServer = configuration["KafkaConfiguration:BootstrapServer"] ?? string.Empty;
        _groupId = configuration["KafkaConfiguration:GroupId"] ?? string.Empty;
        _logger = logger;
    }


    public async Task ProducerAsync<TMessage>(string topic, TMessage message)
    {
        var schemaRegistryConfig = new SchemaRegistryConfig { Url = _kafkaServer };
        
        var _config = new ProducerConfig { BootstrapServers = _kafkaServer };
        
        using var schemaRegistry = new CachedSchemaRegistryClient(schemaRegistryConfig);
        
        using var producer = new ProducerBuilder<Null, TMessage>(_config)
            //.SetValueSerializer(new AvroSerializer<TMessage>(schemaRegistry))
            .SetValueSerializer(new CustomSerializer<TMessage>())
            .Build();
        
        try
        {
            var result = await producer.ProduceAsync(topic, new Message<Null, TMessage>{ Value = message });
            _logger.LogInformation(($"[Event][Producer] Status: {result.Status} | Offset: {result.Offset} | Body: {JsonSerializer.Serialize(result.Message.Value)}"));
        }
        catch (Exception ex)
        {
            producer.Dispose();
            _logger.LogCritical(ex.ToString());
        }
    }   

    public async void ConsumerAsync<TMessage>(string topic, Func<TMessage, Task> function, CancellationToken stoppingToken)
    {
        await Task.Yield();
        
        var config = new ConsumerConfig
        {
            BootstrapServers = _kafkaServer,
            GroupId = _groupId,
            AutoOffsetReset = AutoOffsetReset.Earliest
        };

        using var consumer = new ConsumerBuilder<Ignore, TMessage>(config)
            .SetValueDeserializer(new CustomDeserializer<TMessage>())
            .Build();
        
        consumer.Subscribe(topic);

        try
        {
            do
            {
                var message = consumer.Consume(stoppingToken);
                
                _logger.LogInformation(($"[Event][Consumer]: {message.Value.ToString()} | Offset: {message.Offset} | Body: {JsonSerializer.Serialize(message.Value)}"));
            
                await function(message.Value);
                
            } while (!stoppingToken.IsCancellationRequested);
        }
        catch (Exception ex)
        {
            consumer.Dispose();
            _logger.LogCritical(ex.ToString());
        }
    }
}