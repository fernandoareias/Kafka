namespace Kafka.MessageBus.Interfaces;

public interface IMessageBus
{
    Task ProducerAsync<TMessage>(string topic, TMessage message);
    void ConsumerAsync<TMessage>(string topic, Func<TMessage, Task> function, CancellationToken stoppingToken);
}