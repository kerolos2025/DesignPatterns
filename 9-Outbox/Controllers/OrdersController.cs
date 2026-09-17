using _9_Outbox.Data;
using _9_Outbox.Events;
using _9_Outbox.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace _9_Outbox.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly AppDbContext _db;

        public OrdersController(AppDbContext db)
        {
            _db = db;
        }

        [HttpPost]
        public async Task<IActionResult> Create(string productName,decimal amount)
        {
            await using var transaction = await _db.Database.BeginTransactionAsync();

            var order = new Order
            {
                Id = Guid.NewGuid(),
                ProductName = productName,
                Amount = amount,
                CreatedAt = DateTime.UtcNow
            };

            _db.Orders.Add(order);

            var orderCreatedEvent = new OrderCreatedEvent
            {
                OrderId = order.Id,
                ProductName = order.ProductName,
                Amount = order.Amount
            };

            var outboxMessage = new OutboxMessage
            {
                Id = Guid.NewGuid(),
                Type = nameof(OrderCreatedEvent),
                Payload = JsonSerializer.Serialize(orderCreatedEvent),
                CreatedAt = DateTime.UtcNow
            };

            _db.OutboxMessages.Add(outboxMessage);

            await _db.SaveChangesAsync();

            await transaction.CommitAsync();

            return Ok(order);
        }
    }
}
