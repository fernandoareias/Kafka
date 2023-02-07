using System.ComponentModel.DataAnnotations;
using Kafka.MessageBus;

namespace Kafka.API.Models;

public class User 
{
    protected User()
    {
        
    }
    public User(string nome)
    {
        Nome = nome;
    }
    
    public Guid Id { get; protected set; } = Guid.NewGuid();
    public string Nome { get; set; } = null!;
}