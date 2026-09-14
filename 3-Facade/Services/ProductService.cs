namespace _3_Facade.Services
{
    public class ProductService : IProductService
    {
        private readonly ILogger<ProductService> _logger;

        public ProductService(ILogger<ProductService> logger)
        {
            _logger = logger;
        }

        public void CheckProduct()
        {
            _logger.LogInformation("@@@@@@Product checked@@@@@@");
        }
    }
}
