namespace _2_Decorator
{
    public class LoggingOrderService : IOrderService
    {
        private readonly IOrderService _orderService;
        private readonly ILogger<LoggingOrderService> _logger;

        public LoggingOrderService(
            IOrderService orderService,
            ILogger<LoggingOrderService> logger)
        {
            _orderService = orderService;
            _logger = logger;
        }
        public string GetOrder()
        {
            _logger.LogInformation("#########GetOrder started#######");

            var result = _orderService.GetOrder();

            _logger.LogInformation("GetOrder result: {Result}", result);
            _logger.LogInformation("#########GetOrder finished#######");

            return result;
        }
    }
}
