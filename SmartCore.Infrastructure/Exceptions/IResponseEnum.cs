using System;
using System.Collections.Generic;
using System.Text;

namespace SmartCore.Infrastructure.Exceptions
{
    public interface IResponseEnum
    {
        /**
         * 错误编码
         * @return 错误编码
         */
        int GetCode();

        /**
         * 错误信息
         * @return 错误信息
         */
        string GetMessage();
    } 
}
