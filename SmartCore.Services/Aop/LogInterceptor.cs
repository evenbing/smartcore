using Castle.DynamicProxy;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SmartCore.Services.Aop
{
    public class LogAOP : IInterceptor
    {
        public LogAOP(ILogger<LogAOP> logger)
        {
            _logger = logger;
        }
        private readonly ILogger<LogAOP> _logger;
        public async void Intercept(IInvocation invocation)
        {
            var dataIntercept = "" +
               $"【当前执行方法】：{ invocation.Method.Name} \r\n" +
               $"【携带的参数有】： {JsonConvert.SerializeObject(invocation.Arguments)}\r\n";

            try
            {
                //执行当前方法   
                invocation.Proceed();

                var returnType = invocation.Method.ReturnType;
                //异步方法
                if (IsAsyncMethod(invocation.Method))
                {

                    if (returnType != null && returnType == typeof(Task))
                    {
                        //等待方法返回的Task
                        Func<Task> res = async () => await (Task)invocation.ReturnValue;

                        invocation.ReturnValue = res();
                    }
                    else //Task<TResult>
                    {
                        var returnType2 = invocation.Method.ReflectedType;//获取返回类型

                        if (returnType2 != null)
                        {
                            var resultType = invocation.Method.ReturnType.GetGenericArguments()[0];

                            MethodInfo methodInfo = typeof(LogAOP).GetMethod("HandleAsync", BindingFlags.Instance | BindingFlags.Public);

                            var mi = methodInfo.MakeGenericMethod(resultType);
                            invocation.ReturnValue = mi.Invoke(this, new[] { invocation.ReturnValue });
                        }
                    }

                    var type = invocation.Method.ReturnType;
                    var resultProperty = type.GetProperty("Result");

                    if (resultProperty != null)
                        dataIntercept += ($"【执行完成结果】：{JsonConvert.SerializeObject(resultProperty.GetValue(invocation.ReturnValue))}");
                }
                //同步方法
                else
                {
                    if (returnType != null && returnType == typeof(void))
                    {

                    }
                    else
                        dataIntercept += ($"【执行完成结果】：{JsonConvert.SerializeObject(invocation.ReturnValue)}");
                }

                _logger.LogWarning(dataIntercept);

                await Task.Run(() =>
                {
                    Parallel.For(0, 1, e =>
                    {
                        //LogHelper.Log("AOPLog", dataIntercept);
                    });
                });
            }
            catch (Exception ex)
            {
                LogEx(ex, dataIntercept);
            }
        }

        //构造等待返回值的异步方法
        public async Task<T> HandleAsync<T>(Task<T> task)
        {
            var t = await task;

            return t;
        }

        private void LogEx(Exception ex, string dataIntercept)
        {
            if (ex != null)
            {
                //执行的 service 中，捕获异常
                dataIntercept += ($"【执行完成结果】：方法中出现异常：{ex.Message + ex.InnerException}\r\n");

                // 异常日志里有详细的堆栈信息
                Parallel.For(0, 1, e =>
                {
                   // LogHelper.Log("AOPLog", dataIntercept);
                    _logger.LogWarning(dataIntercept);
                });
            }
        }

        /// <summary>
        /// 判断是否异步方法
        /// </summary>
        public static bool IsAsyncMethod(MethodInfo method)
        {
            return (
                method.ReturnType == typeof(Task) ||
                (method.ReturnType.IsGenericType && method.ReturnType.GetGenericTypeDefinition() == typeof(Task<>))
                );
        }
    }
}
