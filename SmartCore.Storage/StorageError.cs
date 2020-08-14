using System;
using System.Collections.Generic;
using System.Text;

namespace SmartCore.Storage
{
    public class StorageError
    {
        public int Code { get; set; }

        public string Message { get; set; }

        public string ProviderMessage { get; set; }
    }
}
