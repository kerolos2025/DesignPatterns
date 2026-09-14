namespace _4_Strategy.Services
{
    public class SmsNotification : INotificationStrategy
    {
        public void Send(string message)
        {
            Console.WriteLine($"Sending SMS: {message}");
        }
    }
}
