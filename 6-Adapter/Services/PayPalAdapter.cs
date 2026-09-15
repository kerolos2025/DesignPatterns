namespace _6_Adapter.Services
{
    public class PayPalAdapter : IPaymentService
    {
        private readonly PayPalService _payPal;

        public PayPalAdapter(PayPalService payPal)
        {
            _payPal = payPal;
        }

        public void Pay(decimal amount)
        {
            _payPal.MakePayment(amount);
        }
    }
}
