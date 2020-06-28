using System;
using System.Net;
namespace SmartCore.ConfigCenter.Apollo.Exceptions
{
    /// <summary>
    /// 
    /// </summary>
    public class ApolloConfigStatusCodeException : Exception
    {
        public ApolloConfigStatusCodeException(HttpStatusCode statusCode, string message)
            : base($"[status code: {statusCode:D}] {message}")
        {
            StatusCode = statusCode;
        }

        public virtual HttpStatusCode StatusCode { get; }
    }
}
