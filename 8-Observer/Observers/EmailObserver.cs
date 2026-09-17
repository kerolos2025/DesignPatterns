using _8_Observer.Events;

namespace _8_Observer.Observers
{
    public class EmailObserver : IOrderObserver
    {
       
        public Task Handle(OrderCreatedEvent Orderevent)
        {
            Console.WriteLine(
                 $"Email sent for Order {Orderevent.OrderId}");

            return Task.CompletedTask;
        }
    }
}
