using Kafka.API.Data;
using Kafka.API.Models;
using Kafka.MessageBus.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Kafka.API.Controllers;

[Route("users")]
public class UserController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IMessageBus _bus;
    public UserController(ApplicationDbContext context, IMessageBus bus)
    {
        _context = context;
        _bus = bus;
    }

  
    
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] string nome)
    {
        var user = new User(nome);
        _context.Users.Add(user);
        bool created = await _context.SaveChangesAsync() > 0;
        
        if(created)
            await _bus.ProducerAsync<User>("user-created", user);
        
        return created ? Created(new Uri($"https://localhost/users/{user.Id.ToString()}"), user) : BadRequest();
    }

}