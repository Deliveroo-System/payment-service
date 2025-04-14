using PayPalCheckoutSdk.Orders;
using PayPalHttp;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using PayPalCheckoutSdk.Core;

namespace Payment_Service.Service
{
    public class PayPalService
    {
        private readonly PayPalHttpClient _client;

        public PayPalService()
        {
            var environment = new SandboxEnvironment("YOUR_CLIENT_ID", "YOUR_CLIENT_SECRET");
            _client = new PayPalHttpClient(environment);
        }

        public async Task<Order> CreatePayment(decimal amount, string currency, string returnUrl, string cancelUrl)
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
                            Value = amount.ToString("F2") // Always format to two decimal places
                        }
                    }
                },
                ApplicationContext = new ApplicationContext
                {
                    ReturnUrl = returnUrl,
                    CancelUrl = cancelUrl,
                    BrandName = "My Payment App",
                    LandingPage = "BILLING",
                    UserAction = "PAY_NOW"
                }
            };

            var request = new OrdersCreateRequest();
            request.Prefer("return=representation");
            request.RequestBody(orderRequest);

            var response = await _client.Execute(request);
            var statusCode = response.StatusCode;

            var result = response.Result<Order>();
            return result;
        }
    }
}
