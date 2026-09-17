using _8_Observer.Events;

namespace _8_Observer.Observers
{
    public interface IOrderObserver
    {
        Task Handle(OrderCreatedEvent Orderevent);
    }
}
