using System;

namespace SmartPantry.ExternalProducts;

// The provider can't be used right now: HTTP error, timeout, no connection, or unreadable response.
public class ExternalProductProviderUnavailableException : Exception
{
    public ExternalProductProviderUnavailableException(string message, Exception? innerException = null)
        : base(message, innerException)
    {
    }
}