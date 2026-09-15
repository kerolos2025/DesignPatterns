namespace _5_Factory.Services
{
    public class SmsNotification : INotification
    {
        public void Send(string message)
        {
            Console.WriteLine($"SMS: {message}");
        }
    }
}
