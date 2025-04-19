using PayPalCheckoutSdk.Core;
using PayPalHttp;

namespace Payment_Service.Service
{
    public class PayPalClient
    {
        public static PayPalEnvironment Environment()
        {
            return new SandboxEnvironment(
                "Aacd_SPuODUux_H7x6evbTSojfds_jToSXaUD4SegYNJE5CM91OWuqbb1-qwkvnEdpMC_YW8zxZGxMdt", 
                "EFXBHqmf1fwp3WYgvJ6R6a8crw3DyEYsrEaw-JY_RO55UvqcjizyBYw-ylMF2wZX2e_BqP2_HXoAew60" 
            );
        }

        
        public static PayPalHttp.HttpClient Client()
        {
            return new PayPalHttpClient(Environment());
        }
    }
}
