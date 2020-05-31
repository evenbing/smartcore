
using System;
using System.Reflection;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using SmartCore.Repository.Base.Impl;

namespace SmartCore.WebApi
{
    /// <summary>
    /// 
    /// </summary>
    public class AutofacComponent
    {  
        /// <summary>
        /// 
        /// </summary>
        private static IContainer container;
        /// <summary>
        /// 注册组件
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IContainer InitAutoFac(IServiceCollection services)
        {
            var builder = new ContainerBuilder();
            //注册数据库操作类
            services.AddScoped(typeof(BaseRepository<>)); 
            //注册数据操作类（App层）
            builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly());
            builder.Populate(services);
            container = builder.Build();
            return container;
        }
        /// <summary>
        /// 从容器中获取对象(Single)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public static T GetService<T>() where T : class
        {
            return container.Resolve<T>();
        }

        /// <summary>
        /// 获取服务(请求生命周期内)
        /// </summary>
        /// <typeparam name="T">接口类型</typeparam>
        /// <returns></returns>
        public static T GetScopeService<T>() where T : class
        {
            return (T)GetService<IHttpContextAccessor>().HttpContext.RequestServices.GetService(typeof(T));
        }

        //public static IServiceProvider Register(IServiceCollection services)
        //{
        //    //实例化Autofac容器
        //    ContainerBuilder builder = new ContainerBuilder();
        //    //将collection中的服务填充到Autofac
        //    builder.Populate(services);
        //    //注册ServiceModule组件[第五步新添加]
        //    builder.RegisterModule<ServiceModule>();
        //    //注册RepositoryModule组件[第五步新添加]
        //    builder.RegisterModule<RepositoryModule>();
        //    //注册InstanceModule组件[第三步已注册]
        //    builder.RegisterModule<InstanceModule>();
        //    //创建容器
        //    IContainer container = builder.Build();
        //    //第三方容器接管Core内置的DI容器
        //    return new AutofacServiceProvider(container);
        //} 
    }
}
