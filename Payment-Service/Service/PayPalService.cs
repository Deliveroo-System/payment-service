using Microsoft.Extensions.Configuration;
using PayPalCheckoutSdk.Core;
using PayPalCheckoutSdk.Orders;
using PayPalHttp;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Payment_Service.Service
{
    public class PayPalService
    {
        private readonly IConfiguration _config;

        public PayPalService(IConfiguration config)
        {
            _config = config;
        }

        private PayPalHttpClient GetClient()
        {
            var environment = new SandboxEnvironment(
                _config["PayPal:ClientId"],
                _config["PayPal:ClientSecret"]
            );

            return new PayPalHttpClient(environment);
        }

        public async Task<string> CreateOrder(decimal amount, string currency)
        {
            var request = new OrdersCreateRequest();
            request.Prefer("return=representation");

            request.RequestBody(new OrderRequest
            {
                CheckoutPaymentIntent = "CAPTURE",
                PurchaseUnits = new List<PurchaseUnitRequest>
                {
                    new PurchaseUnitRequest
                    {
                        AmountWithBreakdown = new AmountWithBreakdown
                        {
                            CurrencyCode = currency,
                            Value = amount.ToString("F2")
                        }
                    }
                },
                ApplicationContext = new ApplicationContext
                {
                    ReturnUrl = "https://example.com/success",  // replace with frontend URL
                    CancelUrl = "https://example.com/cancel"    // replace with frontend cancel URL
                }
            });

            var response = await GetClient().Execute(request);
            var result = response.Result<Order>();
            return result.Id;
        }

        public async Task<string> GetApprovalLink(string orderId)
        {
            var request = new OrdersGetRequest(orderId);
            var response = await GetClient().Execute(request);
            var order = response.Result<Order>();
            return order.Links.FirstOrDefault(link => link.Rel == "approve")?.Href;
        }

        public async Task<bool> CaptureOrder(string orderId)
        {
            var request = new OrdersCaptureRequest(orderId);
            request.RequestBody(new OrderActionRequest());

            var response = await GetClient().Execute(request);
            return response.StatusCode == HttpStatusCode.Created;
        }
    }
}
