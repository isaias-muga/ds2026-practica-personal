using System;

namespace SmartPantry.ExternalProducts;

// The provider is throttling us (HTTP 429).
public class ExternalProductProviderRateLimitedException : Exception
{
    public ExternalProductProviderRateLimitedException()
        : base("The external product catalog is rate limiting requests.")
    {
    }
}