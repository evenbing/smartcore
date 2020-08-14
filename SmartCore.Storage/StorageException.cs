using System;
using System.Collections.Generic;
using System.Text;

namespace SmartCore.Storage
{
    public class StorageException : Exception
    {
        public StorageException(StorageError error, Exception ex) : base(error.Message, ex)
        {
            ErrorCode = error.Code;
            ProviderMessage = ex?.Message;
        }

        public int ErrorCode { get; private set; }

        public string ProviderMessage { get; set; }
    }
}
