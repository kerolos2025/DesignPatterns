using _8_Observer.Events;
using _8_Observer.Observers;

namespace _8_Observer.Services
{
    public class OrderService
    {
        private readonly IEnumerable<IOrderObserver> _observers;

        public OrderService(IEnumerable<IOrderObserver> observers)
        {
            _observers = observers;
        }

        public async Task CreateOrder(int orderId)
        {
            Console.WriteLine("Creating Order...");

            var Orderevent = new OrderCreatedEvent(orderId);

            foreach (var observer in _observers)
            {
                await observer.Handle(Orderevent);
            }
        }
    }
}
