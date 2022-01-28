using System;
using System.Net;

namespace Payment.Services
{
    public class WavesApiException : Exception
    {
        public HttpStatusCode StatusCode;

        public WavesApiException(HttpStatusCode statusCode, string message) : base(message)
        {
            StatusCode = statusCode;
        }
    }
}