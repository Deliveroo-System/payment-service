using PayPalCheckoutSdk.Core;
using System;

public class PayPalClient
{
    private readonly IConfiguration _config;

    public PayPalClient(IConfiguration config)
    {
        _config = config;
    }

    public PayPalEnvironment Environment()
    {
        var clientId = _config["PayPal:ClientId"];
        var secret = _config["PayPal:Secret"];
        return new SandboxEnvironment(clientId, secret);
    }

    public PayPalHttpClient Client()
    {
        return new PayPalHttpClient(Environment());
    }
}
