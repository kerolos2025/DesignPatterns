using _4_Strategy.Services;

namespace _4_Strategy
{
    public class NotificationService
    {
        private readonly INotificationStrategy _strategy;

        public NotificationService(INotificationStrategy strategy)
        {
            _strategy = strategy;
        }

        public void Send(string message)
        {
            _strategy.Send(message);
        }
    }
}
