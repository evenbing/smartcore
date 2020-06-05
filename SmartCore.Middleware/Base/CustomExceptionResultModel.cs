using Microsoft.AspNetCore.Mvc;
using SmartCore.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartCore.Middleware
{

    public class CustomExceptionResultModel : ApiResultModels
    {
        /// <summary>
        /// 异常处理 
        /// </summary>
        /// <param name="code"></param>
        /// <param name="exception"></param>
        public CustomExceptionResultModel(int code, Exception exception)
        {
            this.code = code;
            this.message = exception.InnerException != null ?
                exception.InnerException.Message :
                exception.Message;
            this.data = exception.ToString();

        }
    }
    public class CustomExceptionResult : ObjectResult
    {
        public CustomExceptionResult(int code, Exception exception)
                : base(new CustomExceptionResultModel(code, exception))
        {
            //StatusCode = code;
        }

    }
    }
