namespace _5_Factory.Services
{
    public class EmailNotification : INotification
    {
        public void Send(string message)
        {
            Console.WriteLine($"Email: {message}");
        }
    }
}
