using System;
using System.Collections.Generic;
using System.Text;
using Castle.DynamicProxy;
namespace SmartCore.Services.Aop
{
    public class RepeatSubmitInterceptor : IInterceptor
    {
        /// <summary>
        /// 默认1s钟以内算重复提交
        /// </summary>
        public long Timeout { get; set; }

        public void Intercept(IInvocation invocation)
        { 
        
        }
    }
}
