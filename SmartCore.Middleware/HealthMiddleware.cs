using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SmartCore.Middleware
{
    /// <summary>
    /// 
    /// </summary>
    public static class HealthMiddlewareExtension
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="app"></param>
        public static void UseHealth(this IApplicationBuilder app)
        {
            app.UseMiddleware<HealthMiddleware>();
        }
    }
    

    public class HealthMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly string _healthPath = "/health";

        public HealthMiddleware(RequestDelegate next, IConfiguration configuration)
        {
            this._next = next;
            var healthPath = configuration["Consul:HealthPath"];
            if (!string.IsNullOrEmpty(healthPath))
            {
                this._healthPath = healthPath;
            }
        }

        /// <summary>
        /// 监控检查可以返回更多的信息，例如服务器资源信息
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        public async Task Invoke(HttpContext httpContext)
        {
            if (httpContext.Request.Path == this._healthPath)
            {
                httpContext.Response.StatusCode = (int)HttpStatusCode.OK;
                await httpContext.Response.WriteAsync("I'm OK!");
            }
            else
                await this._next(httpContext);
        }
    }
}
