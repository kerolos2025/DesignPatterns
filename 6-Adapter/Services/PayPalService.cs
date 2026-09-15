namespace _6_Adapter.Services
{
    public class PayPalService
    {
        public void MakePayment(decimal amount)
        {
            Console.WriteLine($"Paid {amount} using PayPal");
        }
    }
}
