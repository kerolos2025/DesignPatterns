namespace _4_Strategy.Services
{
    public class EmailNotification : INotificationStrategy
    {
        public void Send(string message)
        {
            Console.WriteLine($"Sending Email: {message}");
        }
    }
}
