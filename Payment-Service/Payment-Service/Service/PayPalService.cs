using System;
using PayPal.Api;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace Payment_Service.Service
{
    public class PayPalService
    {
        private readonly string _clientId;
        private readonly string _clientSecret;
        private readonly string _mode;

        public PayPalService(IConfiguration config)
        {
            _clientId = config["PayPal:ClientId"];
            _clientSecret = config["PayPal:ClientSecret"];
            _mode = config["PayPal:Mode"];
        }

        private APIContext GetAPIContext()
        {
            var config = new Dictionary<string, string> { { "mode", _mode } };
            var accessToken = new OAuthTokenCredential(_clientId, _clientSecret, config).GetAccessToken();
            return new APIContext(accessToken) { Config = config };
        }

        public async Task<Payment> CreatePayment(decimal amount, string currency, string returnUrl, string cancelUrl)
        {
            var apiContext = GetAPIContext();
            var payment = new Payment
            {
                intent = "sale",
                payer = new Payer { payment_method = "paypal" },
                transactions = new List<Transaction>
            {
                new Transaction
                {
                    amount = new Amount { total = amount.ToString("F2"), currency = currency },
                    description = "Food Order Payment"
                }
            },
                redirect_urls = new RedirectUrls { return_url = returnUrl, cancel_url = cancelUrl }
            };
            return await Task.Run(() => payment.Create(apiContext));
        }
    }
}
