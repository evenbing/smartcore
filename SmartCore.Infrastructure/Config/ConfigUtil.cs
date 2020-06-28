using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SmartCore.Infrastructure.Config
{
    public class ConfigUtil
    {
        /// <summary>
        /// 
        /// </summary>
        private static IConfiguration configuration;
        static ConfigUtil()
        {
            //var config = new ConfigurationBuilder()
            //    .SetBasePath(Directory.GetCurrentDirectory())//设置基础路径
            //    .AddJsonFile($"appsettings.json", true, true)//加载配置文件
            //    .AddJsonFile($"appsettings.{EnvironmentName.Development}.json", true, true)
            //    .Build();
            //在当前目录或者根目录中寻找appsettings.json文件
            var fileName = "appsettings.json";

            var directory = AppContext.BaseDirectory;
            directory = directory.Replace("\\", "/");

            var filePath = $"{directory}/{fileName}";
            if (!File.Exists(filePath))
            {
                var length = directory.IndexOf("/bin");
                filePath = $"{directory.Substring(0, length)}/{fileName}";
            }

            var builder = new ConfigurationBuilder()
                .AddJsonFile(filePath, false, true);
            //var builder = new ConfigurationBuilder();
            configuration = builder.Build();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="config"></param>
        public static void Configure(IConfiguration config)
        {
            configuration = config;
        }
        /// <summary>
        /// 根据配置文件键读取对应的值
        /// </summary>
        /// <param name="Key"></param>
        /// <returns></returns>
        public static string GetConfigValueByKey(string Key)
        {
            var value = configuration[Key];
            return value;
        }
        /// <summary>
        /// 根据配置文件键读取对应的值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public static T GetAppSettings<T>(string key) where T : class, new()
        {
            var appconfig = new ServiceCollection()
             .AddOptions()
             .Configure<T>(configuration.GetSection(key))
             .BuildServiceProvider()
             .GetService<IOptions<T>>()
             .Value;
            return appconfig;
        }
    }
}
