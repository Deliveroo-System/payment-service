using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using PayPalCheckoutSdk.Core;
using PayPalCheckoutSdk.Orders;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace Payment_Service.Service
{
    public class PayPalService
    {
        private readonly IConfiguration _config;
        private readonly ILogger<PayPalService> _logger;

        public PayPalService(IConfiguration config, ILogger<PayPalService> logger)
        {
            _config = config;
            _logger = logger;
        }

        private PayPalEnvironment GetPayPalEnvironment()
        {
            string clientId = _config["PayPal:ClientId"];
            string clientSecret = _config["PayPal:ClientSecret"];
            string mode = _config["PayPal:Mode"]; // "sandbox" or "live"

            if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(clientSecret))
            {
                _logger.LogError("PayPal credentials are missing.");
                throw new Exception("PayPal credentials are missing.");
            }

            if (mode != "sandbox" && mode != "live")
            {
                _logger.LogError("Invalid PayPal mode. It should be 'sandbox' or 'live'.");
                throw new Exception("Invalid PayPal mode. It should be 'sandbox' or 'live'.");
            }

            return mode == "sandbox"
                ? new SandboxEnvironment(clientId, clientSecret)
                : new LiveEnvironment(clientId, clientSecret);
        }


        private PayPalHttpClient GetPayPalClient()
        {
            return new PayPalHttpClient(GetPayPalEnvironment());
        }

        public async Task<string> CreatePayPalOrder(decimal amount, string currency)
        {
            try
            {
                var request = new OrdersCreateRequest();
                request.Prefer("return=representation");

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
                        Value = amount.ToString("F2")
                    }
                }
            }
                };

                request.RequestBody(orderRequest);

                var client = GetPayPalClient();
                var response = await client.Execute(request);

                if (response.StatusCode != System.Net.HttpStatusCode.OK || response.Result<Order>() == null)
                {
                    var responseBody = Newtonsoft.Json.JsonConvert.SerializeObject(response);
                    _logger.LogError($"Failed to create PayPal order. Response Status: {response.StatusCode}, Body: {responseBody}");
                    throw new Exception($"PayPal order creation failed. Response status: {response.StatusCode}. Response: {responseBody}");
                }

                var result = response.Result<Order>();
                _logger.LogInformation($"PayPal order created successfully. Order ID: {result.Id}");
                return result.Id;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error creating PayPal order: {ex.Message}", ex);
                throw new Exception("Error creating PayPal order.", ex);
            }
        }




        public async Task<string> CapturePayPalPayment(string orderId)
        {
            try
            {
                if (string.IsNullOrEmpty(orderId))
                {
                    _logger.LogError("PayPal Order ID is null or empty.");
                    throw new ArgumentException("PayPal Order ID cannot be null or empty.");
                }

                var request = new OrdersCaptureRequest(orderId);
                request.RequestBody(new OrderActionRequest());

                var client = GetPayPalClient();
                var response = await client.Execute(request);
                var result = response.Result<Order>();

                if (result == null)
                {
                    _logger.LogError("PayPal capture response result is null.");
                    throw new Exception("Failed to capture PayPal payment.");
                }

                _logger.LogInformation($"PayPal payment captured. Order ID: {orderId}, Status: {result.Status}");
                return result.Status; // Should return "COMPLETED" if successful
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error capturing PayPal payment: {ex.Message}", ex);
                throw new Exception("Error capturing PayPal payment.", ex);
            }
        }
    }
}
