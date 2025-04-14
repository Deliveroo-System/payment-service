using PayPalCheckoutSdk.Core;
using PayPalCheckoutSdk.Orders;
using PayPalHttp; // correct!
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Payment_Service.Controller
{
    public class PayPalService
    {
        private readonly PayPalEnvironment _environment;
        private readonly PayPalHttpClient _client;

        public PayPalService()
        {
            _environment = new SandboxEnvironment("Aacd_SPuODUux_H7x6evbTSojfds_jToSXaUD4SegYNJE5CM91OWuqbb1-qwkvnEdpMC_YW8zxZGxMdt", "EFXBHqmf1fwp3WYgvJ6R6a8crw3DyEYsrEaw-JY_RO55UvqcjizyBYw-ylMF2wZX2e_BqP2_HXoAew60");
            _client = new PayPalHttpClient(_environment);
        }

        public async Task<Order> CreatePayment(decimal totalAmount, string currency, string returnUrl, string cancelUrl)
        {
            var orderRequest = new OrderRequest()
            {
                CheckoutPaymentIntent = "CAPTURE",
                PurchaseUnits = new List<PurchaseUnitRequest>
                {
                    new PurchaseUnitRequest
                    {
                        AmountWithBreakdown = new AmountWithBreakdown
                        {
                            CurrencyCode = currency,
                            Value = totalAmount.ToString("F2")
                        }
                    }
                },
                ApplicationContext = new ApplicationContext
                {
                    ReturnUrl = returnUrl,
                    CancelUrl = cancelUrl
                }
            };

            var request = new OrdersCreateRequest();
            request.Prefer("return=representation");
            request.RequestBody(orderRequest);

            var response = await _client.Execute(request);
            var result = response.Result<Order>();
            return result;
        }
    }
}
