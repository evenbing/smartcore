using System;
using System.Collections.Generic;
using System.Text;

namespace SmartCore.ConfigCenter.Apollo.Exceptions
{
    public class ApolloConfigException : Exception
    {
        public ApolloConfigException(string message) : base(message) { }
        public ApolloConfigException(string message, Exception ex) : base(message, ex) { }
    }
}
