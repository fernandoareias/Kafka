using Kafka.API.Models;
using Kafka.MessageBus.Interfaces;

namespace Kafka.API.BackgroundServices;

public class UserCreatedService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;

    public UserCreatedService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var scope = _serviceProvider.CreateScope();

        var messageBus = scope.ServiceProvider.GetService<IMessageBus>() ?? throw new NullReferenceException(nameof(MessageBus));
        
        messageBus.ConsumerAsync<User>("user-created", ProcessUser, stoppingToken);
    }
    
    private Task ProcessUser(User user)
    {
        Console.WriteLine($"Usuario {user.Nome} processado.");

        return Task.CompletedTask;
    }
}