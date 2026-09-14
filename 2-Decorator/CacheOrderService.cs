namespace _2_Decorator
{
    public class CacheOrderService : IOrderService
    {
        private readonly IOrderService _inner;

        private readonly ILogger<CacheOrderService> _logger;


        public CacheOrderService(IOrderService inner, ILogger<CacheOrderService> logger)
        {
            _inner = inner;
            _logger = logger;
        }
        public string GetOrder()
        {
            _logger.LogInformation("------------Cache started----------");


            var result = _inner.GetOrder();

            _logger.LogInformation("------------Cache Ended----------");


            return result;
        }
    }
}
