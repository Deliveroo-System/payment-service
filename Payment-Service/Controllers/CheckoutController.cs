//using Microsoft.AspNetCore.Mvc;
//using System.Runtime.CompilerServices;
//using System.Text;

//namespace Payment_Service.Controllers
//{
//    public class CheckoutController : Controller
//    {

//        private string Paypalclient { get; set; } = "";
//        private string Paypalsecret { get; set; } = "";
//        private string Paypalurl { get; set; } = "";

//        public CheckoutController(IConfiguration configuration)
//        {
//            Paypalclient = configuration["PayPal:ClientId"];
//            Paypalsecret = configuration["PayPal:ClientSecret"];
//            Paypalurl = configuration["PayPal:Url"]!;
//        }

//        public IActionResult Index() 
//        {

//            return View();
//        }

//        private string GetPaypalAccessToken() { 
//            string accessToken = "";

//            string credentials64 =
//                Convert.ToBase64String(Encoding.UTF8.GetBytes(PaypalClientId + ":" + PaypalSecret));

//            client.DefaultRequestHeaders.Add("Authorization", "Basic" + credentials64);

//            var requestMessage = new HttpRequestMessage(HttpMethod.Post, url);
//            requestMessage.Content = newmStringContent("grant_type=")

//            return accessToken;
//        }
//    }
//}
