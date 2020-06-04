using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SmartCore.Models;

namespace SmartCore.Middleware
{
    /// <summary>
    /// ASP.NET Core WebApi 返回统一格式参数
    /// </summary>
    public class WebApiResultMiddleware : ActionFilterAttribute
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        public override void OnResultExecuting(ResultExecutingContext context)
        {
            string traceIdentifier = context.HttpContext?.TraceIdentifier;
            if (context.Result is ValidationFailedResult)
            {
                var objectResult = context.Result as ObjectResult;
                context.Result = objectResult;
            }
            else
            {
                //根据实际需求进行具体实现
                if (context.Result is ObjectResult)
                {
                    var objectResult = context.Result as ObjectResult;
                    if (objectResult.Value == null)
                    {
                        context.Result = new OkObjectResult(new ApiResultModels { code = 404, message = "未找到资源" });
                    }
                    else
                    {
                        context.Result = new OkObjectResult(new ApiResultModels { code = 200, message = "操作成功", data = objectResult.Value });
                    }
                }
                else if (context.Result is EmptyResult)
                {
                    context.Result = new OkObjectResult(new ApiResultModels{ code = 404, message = "未找到资源"});
                }
                else if (context.Result is ContentResult)
                {
                    context.Result = new OkObjectResult(new ApiResultModels { code = 200, message = "调用接口成功", data = (context.Result as ContentResult)?.Content});
                }
                else if (context.Result is StatusCodeResult)
                {
                    context.Result = new OkObjectResult(new ApiResultModels { code = (context.Result as StatusCodeResult).StatusCode, message = "" });
                }
            }
        }

    }

    //    Startup添加对应配置：

    //public void ConfigureServices(IServiceCollection services)
    //    {
    //        services.AddMvc(options =>
    //        {
    //            options.Filters.Add(typeof(WebApiResultMiddleware));
    //            options.RespectBrowserAcceptHeader = true;
    //        });

}
