namespace _3_Facade.Services
{
    public class OrderService : IOrderService
    {
        private readonly ILogger<OrderService> _logger;

        public OrderService(ILogger<OrderService> logger)
        {
            _logger = logger;
        }

        public void CreateOrder()
        {
            _logger.LogInformation("--------Order created--------");
        }
    }
}
