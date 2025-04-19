using PayPalCheckoutSdk.Orders;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Payment_Service.Service
{
    public class PayPalService
    {
        public async Task<Order> CreatePayment(decimal amount, string currency, string returnUrl, string cancelUrl)
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
                    ReturnUrl = returnUrl,
                    CancelUrl = cancelUrl,
                    BrandName = "YourBrand",
                    LandingPage = "LOGIN",
                    UserAction = "PAY_NOW"
                }
            });

            var response = await PayPalClient.Client().Execute(request);
            return response.Result<Order>();
        }

        public async Task<Order> CapturePayment(string token)
        {
            var request = new OrdersCaptureRequest(token);
            request.RequestBody(new OrderActionRequest());

            var response = await PayPalClient.Client().Execute(request);
            return response.Result<Order>();
        }
    }
}
