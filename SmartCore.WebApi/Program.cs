using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Web;
using SmartCore.ConfigCenter.Apollo;

namespace SmartCore.WebApi
{
    /// <summary>
    /// 
    /// </summary>
    public class Program
    {
        //private static Logger logger = LogManager.GetCurrentClassLogger();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
           // SmartCore.Infrastructure.LogManager.Info("Current Thead Id:{0}", Thread.CurrentThread.ManagedThreadId);
            //logger.Info(string.Format("Current Thead Id:{0}", Thread.CurrentThread.ManagedThreadId));
            CreateHostBuilder(args).Build().Run();
        }
        /// <summary>
        ///WebHost ： 承载Web应用的宿主 创建作为应用宿主的WebHost；WebHostBuilder ：WebHost的构建者；
        /// </summary>
        /// <param name="args"></param>
        /// <remark>.NET CORE 内置一个IOC容器 用第三方IOC容器Autofac替代内置的</remark>
        /// <returns></returns>
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
              .ConfigureAppConfiguration((hostingContext, builder) =>
                { 
                    var env = hostingContext.HostingEnvironment;
                    //加载appsettings.json文件 使用模板创建的项目，会生成一个配置文件，配置文件中包含Logging的配置项
                    builder.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                       .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true);
                    //默认 namespace: application
                    // .AddApollo(builder.Build().GetSection("apollo"))
                    //.AddDefault()
                    //ApolloConfig.Configuration = builder.Build();
                })
            .ConfigureWebHostDefaults(webBuilder =>
            {
                    webBuilder.UseStartup<Startup>().UseNLog();
            }).UseServiceProviderFactory(new AutofacServiceProviderFactory());

        //.UseKestrel(options =>
        //            {
        //    //options.ListenUnixSocket("/tmp/kestrel-server.sock");
        //    //options.ListenUnixSocket("/tmp/kestrel-test.sock", listenOptions =>
        //    //{
        //    //    listenOptions.UseHttps("testCert.pfx", "testpassword");
        //    //}); 
        //    // Set properties and call methods on options
        //    //为整个应用设置并发打开的最大 TCP 连接数,默认情况下，最大连接数不受限制 (NULL)
        //    //options.Limits.MaxConcurrentConnections = 100;
        //    //对于已从 HTTP 或 HTTPS 升级到另一个协议（例如，Websocket 请求）的连接，有一个单独的限制。 连接升级后，不会计入 MaxConcurrentConnections 限制
        //    //options.Limits.MaxConcurrentUpgradedConnections = 100;
        //    //最大请求体大小Maximum request body size 缺省值为30,000,000byte, 大约是28.6MB。
        //    options.Limits.MaxRequestBodySize = 10 * 1024;
        //    //最小请求提数据率Minimum request body data rate 缺省值为30 to 240 bytes/second with a 5 second grace period. https://docs.microsoft.com/zh-cn/dotnet/api/microsoft.aspnetcore.server.kestrel.core.kestrelserverlimits.minrequestbodydatarate?view=aspnetcore-2.1
        //    options.Limits.MinRequestBodyDataRate = null;//赋值null 为了解决大概并发在 50 个时会发生Reading the request body timed out due to data arriving too slowly. See MinRequestBodyDataRate这个异常
        //                                                 //new MinDataRate(bytesPerSecond: 100, gracePeriod: TimeSpan.FromSeconds(10));
        //    options.Limits.MinResponseDataRate =
        //        new MinDataRate(bytesPerSecond: 100, gracePeriod: TimeSpan.FromSeconds(10));
        //    //获取或设置保持活动状态超时。 
        //    //option.Limits.KeepAliveTimeout = TimeSpan.FromMinutes(20);
        //    //option.Limits.RequestHeadersTimeout = TimeSpan.FromMinutes(20);
        //    //options.Listen(IPAddress.Loopback, 5000);
        //    //options.Listen(IPAddress.Loopback, 5001, listenOptions =>
        //    //{
        //    //    listenOptions.UseHttps("testCert.pfx", "testPassword");
        //    //});

        //})

    }
}
