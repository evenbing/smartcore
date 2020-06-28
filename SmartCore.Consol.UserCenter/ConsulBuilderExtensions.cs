using Consul;
using Microsoft.AspNetCore.Builder; 
using Microsoft.Extensions.Hosting;
using System;

namespace SmartCore.Consol.Market
{
    public static class ConsulBuilderExtensions
    {
        public static IApplicationBuilder RegisterConsul(this IApplicationBuilder app, IHostApplicationLifetime lifetime, ConsulOption consulOption)
        {
            var consulClient = new ConsulClient(x =>
            {
                // consul 服务地址
                x.Address = new Uri(consulOption.Address);
            }); 
            var registration = new AgentServiceRegistration()
            {
                Check = new AgentServiceCheck()
                { 
                    DeregisterCriticalServiceAfter = TimeSpan.FromSeconds(5),//服务启动多久后注册
                    Interval = TimeSpan.FromSeconds(10),//健康检查时间间隔或者称为心跳间隔
                    HTTP = consulOption.ServiceHealthCheck,//健康检查地址
                    Timeout = TimeSpan.FromSeconds(5)
                },
                ID = string.Concat(consulOption.ServiceName, "_",Guid.NewGuid().ToString()),
                Name = consulOption.ServiceName,// 服务名
                Address = consulOption.ServiceIP, // 服务绑定IP
                Port = consulOption.ServicePort, // 服务绑定端口
                Tags = new[] { $"urlprefix-/{consulOption.ServiceName}" }//添加 urlprefix-/servicename 格式的 tag 标签，以便 Fabio 识别
            };

            // 服务启动时注册，内部实现其实就是使用 Consul API 进行注册（HttpClient发起）
            consulClient.Agent.ServiceRegister(registration).Wait();

            // 应用程序终止时，服务取消注册
            lifetime.ApplicationStopping.Register(() =>
            {
                consulClient.Agent.ServiceDeregister(registration.ID).Wait();////服务停止时取消注册
            });
            return app;
        }
    }
}
