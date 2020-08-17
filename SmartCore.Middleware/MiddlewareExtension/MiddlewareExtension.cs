using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartCore.Middleware
{
    public static class MiddlewareExtensions
    {
        //    public static IApplicationBuilder UseTokenCheck(this IApplicationBuilder builder)
        //    {
        //        return builder.UseMiddleware<TokenCheckMiddleware>();
        //    }
        //}
        public static IApplicationBuilder UseHttpContextMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<HttpContextMiddleware>();
    }
        //    3、中间件注册/引用

        //在启动类Startup.cs的Configure方法中注册/引用中间件

        //public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        //    {

        //        //省略部分代码

        //        app.UseTokenCheck();

        //        app.UseMvc(routes =>
        //        {
        //            //省略路由配置代码
        //        });
    }
}
