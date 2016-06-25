using System;
using System.Net;

namespace Homero.Core.Utility
{
    /// <summary>
    /// WebClient that exposes ResponseUri.
    /// </summary>
    /// <seealso cref="System.Net.WebClient" />
    public class UriWebClient : WebClient
    {
        /// <summary>
        /// Gets the response URI.
        /// </summary>
        /// <value>The response URI.</value>
        public Uri ResponseUri { get; private set; }

        protected override WebResponse GetWebResponse(WebRequest request)
        {
            var response = base.GetWebResponse(request);
            ResponseUri = response.ResponseUri;
            return response;
        }
    }
}