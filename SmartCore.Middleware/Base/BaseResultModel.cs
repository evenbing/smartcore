using System;
using System.Collections.Generic;
using System.Text;

namespace SmartCore.Middleware
{
    public class BaseResultModel
    {
        public BaseResultModel(int code = 0, object data = null, string message = "")
        {
            this.code = code;
            this.data = data;
            this.message = message;
        }
        public int code { get; set; }

        public string message { get; set; }

        public object data { get; set; }

    }
}
