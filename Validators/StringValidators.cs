// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global

using System;

namespace SharpedUtilsCollection.Validators
{
    public class StringValidators
    {
        public static bool IsValidUrl(string url)
        {
            if (!Uri.TryCreate(url, UriKind.Absolute, out var uri))
            {
                return false;
            }

            return uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps;
        }
    }
}
