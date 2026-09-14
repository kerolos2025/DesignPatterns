using _3_Facade.Services;

namespace _3_Facade
{
    public class OrderFacade : IOrderFacade
    {
        private readonly IProductService _productService;
        private readonly IPaymentService _paymentService;
        private readonly IOrderService _orderService;

        public OrderFacade(
            IProductService productService,
            IPaymentService paymentService,
            IOrderService orderService)
        {
            _productService = productService;
            _paymentService = paymentService;
            _orderService = orderService;
        }

        public void CreateOrder()
        {
            _productService.CheckProduct();

            _paymentService.ProcessPayment();

            _orderService.CreateOrder();
        }
    }
}
