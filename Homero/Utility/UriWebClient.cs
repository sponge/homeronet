using System;
using System.Net;

namespace Homero.Utility
{
    /// <summary>
    /// WebClient that exposes ResponseUri.
    /// </summary>
    /// <seealso cref="System.Net.WebClient" />
    public class UriWebClient : WebClient
    {
        Uri _responseUri;

        /// <summary>
        /// Gets the response URI.
        /// </summary>
        /// <value>The response URI.</value>
        public Uri ResponseUri
        {
            get { return _responseUri; }
        }

        protected override WebResponse GetWebResponse(WebRequest request)
        {
            WebResponse response = base.GetWebResponse(request);
            _responseUri = response.ResponseUri;
            return response;
        }
    }
}
