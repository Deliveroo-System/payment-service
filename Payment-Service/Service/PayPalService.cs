using Microsoft.Extensions.Configuration;
using PayPalCheckoutSdk.Core;
using PayPalCheckoutSdk.Orders;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Payment_Service.Service
{
    public class PayPalService
    {
        private readonly PayPalEnvironment _environment;
        private readonly PayPalHttpClient _client;

        public PayPalService(IConfiguration configuration)
        {
            var clientId = configuration["PayPal:ClientId"];
            var clientSecret = configuration["PayPal:ClientSecret"];
            var mode = configuration["PayPal:Mode"];

            if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(clientSecret))
                throw new Exception("PayPal ClientId or ClientSecret not configured");

            _environment = mode?.ToLower() == "live"
                ? new LiveEnvironment(clientId, clientSecret)
                : new SandboxEnvironment(clientId, clientSecret);

            _client = new PayPalHttpClient(_environment);
        }

        public async Task<Order> CreatePayment(decimal amount, string currency, string returnUrl, string cancelUrl)
        {
            var request = new OrdersCreateRequest();
            request.Prefer("return=representation");
            request.RequestBody(new OrderRequest
            {
                CheckoutPaymentIntent = "CAPTURE",
                ApplicationContext = new ApplicationContext
                {
                    ReturnUrl = returnUrl,
                    CancelUrl = cancelUrl,
                    BrandName = "MyCompany",
                    LandingPage = "BILLING",
                    UserAction = "PAY_NOW"
                },
                PurchaseUnits = new List<PurchaseUnitRequest>
                {
                    new PurchaseUnitRequest
                    {
                        AmountWithBreakdown = new AmountWithBreakdown
                        {
                            CurrencyCode = currency.ToUpper(),
                            Value = amount.ToString("F2")
                        }
                    }
                }
            });

            var response = await _client.Execute(request);
            return response.Result<Order>();
        }

        public async Task<Order> CapturePayment(string orderId)
        {
            var request = new OrdersCaptureRequest(orderId);
            request.RequestBody(new OrderActionRequest()); // Empty body is required
            var response = await _client.Execute(request);
            return response.Result<Order>();
        }
    }
}
