using _8_Observer.Events;

namespace _8_Observer.Observers
{
    public class SMSObserver : IOrderObserver
    {
       
        public Task Handle(OrderCreatedEvent Orderevent)
        {
            Console.WriteLine(
                 $"SMS sent for Order {Orderevent.OrderId}");

            return Task.CompletedTask;
        }
    }
}
