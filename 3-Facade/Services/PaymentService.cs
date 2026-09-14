namespace _3_Facade.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly ILogger<PaymentService> _logger;

        public PaymentService(ILogger<PaymentService> logger)
        {
            _logger = logger;
        }

        public void ProcessPayment()
        {
            _logger.LogInformation("########Payment processed########");
        }
    }
}
