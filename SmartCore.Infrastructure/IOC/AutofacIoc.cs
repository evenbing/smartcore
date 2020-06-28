using Autofac;
using Autofac.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartCore.Infrastructure.IOC
{
    public static class AutofacIoc
    { /// <summary>
      /// 默认容器
      /// </summary>
        internal static ContainerBuilder builder { get; set; }
        //private static IContainer container { get; set; }
        public static ContainerBuilder Builder
        {
            get
            {
                if (builder == null)
                {
                    builder = new ContainerBuilder();
                }
                return builder;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="Interface"></typeparam>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TParam"></typeparam>
        /// <returns></returns>
        public static Interface RegisterType<Interface, T>()
        {
            Builder.RegisterType<T>().As<Interface>();
            using (var container = builder.Build())
            {
                return container.Resolve<Interface>();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="Interface"></typeparam>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TParam"></typeparam>
        /// <param name="param"></param>
        /// <param name="paramName"></param>
        /// <returns></returns>
        public static Interface Register<Interface, T, TParam>(TParam param, string paramName)
        {
            Builder.RegisterType<T>().As<Interface>().WithParameter(paramName,param);
            using (var container = builder.Build())
            { 
                return container.Resolve<Interface>();
            }
        }
        /// <summary>
        /// 创建实例
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="name">服务名称</param>
        public static T RegisterType<T>(string name) where T : class
        {
            //var result=  builder.RegisterType<T>();
            //  return result.AsImplementedInterfaces<T>();
            using (var container = builder.Build())
            {
                //using (var scope = container.BeginLifetimeScope())
                //{
                //    return scope.ResolveNamed<T>(nameof(T));
                //}
                // return container.Resolve<T>();//.PropertiesAutowired();//.PropertiesAutowired();
                return container.Resolve<T>();
            }

        }


    }
}
