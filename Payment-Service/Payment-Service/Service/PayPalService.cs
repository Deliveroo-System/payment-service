using System;
using System.Collections.Generic;
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

            if (string.IsNullOrEmpty(_clientId) || string.IsNullOrEmpty(_clientSecret))
            {
                throw new ArgumentException("PayPal ClientId or ClientSecret is missing.");
            }

            try
            {
                // Attempt to retrieve the access token
                var accessToken = new OAuthTokenCredential(_clientId, _clientSecret, config).GetAccessToken();

                // Log the access token for verification
                Console.WriteLine($"AccessToken: {accessToken}");

                var apiContext = new APIContext(accessToken) { Config = config };

                if (apiContext == null)
                {
                    throw new InvalidOperationException("Failed to create APIContext.");
                }

                return apiContext;
            }
            catch (Exception ex)
            {
                // Log or handle the error (e.g., invalid credentials or network issue)
                Console.WriteLine($"Error creating APIContext: {ex.Message}");
                throw;
            }
        }


        public Payment CreatePayment(decimal amount, string currency, string returnUrl, string cancelUrl)
        {
            try
            {
                var apiContext = GetAPIContext();

                if (apiContext == null)
                {
                    throw new InvalidOperationException("APIContext is null.");
                }

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

                // Call Create() directly (without async/await)
                var createdPayment = payment.Create(apiContext);

                return createdPayment;
            }
            catch (Exception ex)
            {
                // Log detailed error
                Console.WriteLine($"Error during payment creation: {ex.Message}");
                throw;
            }
        }





    }
}
