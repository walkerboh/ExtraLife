using System;
using System.Net;

namespace ExtraLife
{
    public class ApiResponseException : Exception
    {
        public ApiResponseException()
        {
        }

        public ApiResponseException(string message) : base(message)
        {
        }

        public ApiResponseException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public HttpStatusCode HttpResponseCode { get; set; }
    }
}