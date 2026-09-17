using _8_Observer.Events;

namespace _8_Observer.Observers
{
    public class LoggingObserver : IOrderObserver
    {

        public Task Handle(OrderCreatedEvent Orderevent)
        {
            Console.WriteLine(
            $"Order {Orderevent.OrderId} created");

            return Task.CompletedTask;
        }
    }
}
