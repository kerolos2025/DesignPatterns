using _9_Outbox.Data;
using Microsoft.EntityFrameworkCore;
using OutboxDemo.Messaging;

namespace _9_Outbox.Workers
{
    public class OutboxWorker : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<OutboxWorker> _logger;

        public OutboxWorker(
            IServiceScopeFactory scopeFactory,
            ILogger<OutboxWorker> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await ProcessMessages(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(
                        ex,
                        "Error processing outbox messages");
                }

                await Task.Delay(
                    TimeSpan.FromSeconds(2),
                    stoppingToken);
            }
        }

        private async Task ProcessMessages( CancellationToken cancellationToken)
        {
            using var scope =
                _scopeFactory.CreateScope();

            var db = scope.ServiceProvider
                .GetRequiredService<AppDbContext>();

            var publisher = scope.ServiceProvider
                .GetRequiredService<RabbitMqPublisher>();

            var messages = await db.OutboxMessages
                .Where(x => x.ProcessedAt == null)
                .OrderBy(x => x.CreatedAt)
                .Take(10)
                .ToListAsync(cancellationToken);

            foreach (var message in messages)
            {
                try
                {
                    await publisher.PublishAsync(
                        message.Type,
                        message.Payload);

                    message.ProcessedAt = DateTime.UtcNow;
                    message.Error = null;

                    await db.SaveChangesAsync(
                        cancellationToken);

                    _logger.LogInformation(
                        "Outbox message {Id} published",
                        message.Id);
                }
                catch (Exception ex)
                {
                    message.RetryCount++;

                    message.Error = ex.Message;

                    await db.SaveChangesAsync(
                        cancellationToken);

                    _logger.LogError(
                        ex,
                        "Failed to publish message {Id}",
                        message.Id);
                }
            }
        }
    }
}
