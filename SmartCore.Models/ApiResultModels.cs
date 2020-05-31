using System;
using System.Collections.Generic;
using System.Text;

namespace SmartCore.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class ApiResultModels
    {
        /// <summary>
        /// 是否成功
        /// </summary>
        public bool Success
        {
            get
            {
                if (!string.IsNullOrEmpty(Code) && Code.Equals("200"))
                {
                    return true;
                }
                return false;
            }
        }
        /// <summary>
        /// 返回的代码 200标识成功，其他请参见错误代码 （枚举类ErrorCodes）
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 返回的中文信息
        /// </summary>
        public string Message { get; set; } 
        /// <summary>
        /// 返回的结果
        /// </summary> 
        public object Data { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public ApiResultModels()
        {

        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="code"></param>
        /// <param name="message"></param>
        /// <param name="title"></param>
        /// <param name="result"></param>
        public ApiResultModels(string code, object result = null, string message = "调用接口成功")
        {
            Code = code;
            Message = message;
            Data = result;
        }
    }
}
