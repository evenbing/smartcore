using System;
using System.Collections.Generic;
using System.Text;

namespace SmartCore.Storage
{
    public class BlobProperties
    {
        public static readonly BlobProperties Empty = new BlobProperties
        {
            Security = BlobSecurity.Private
        };

        public BlobSecurity Security { get; set; }

        public string ContentType { get; set; }
    }
}
