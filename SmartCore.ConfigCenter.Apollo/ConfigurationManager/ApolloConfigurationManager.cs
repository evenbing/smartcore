using SmartCore.ConfigCenter.Apollo.ConfigurationManager.Util;
using SmartCore.ConfigCenter.Apollo.Core;
using SmartCore.ConfigCenter.Apollo.Internals;
using SmartCore.ConfigCenter.Apollo.Spi;
using SmartCore.ConfigCenter.Apollo.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartCore.ConfigCenter.Apollo.ConfigurationManager
{
    /// <summary>
    /// Entry point for client config use
    /// </summary>
 
    [Obsolete("不建议使用，推荐安装包Com.Ctrip.Framework.Apollo.Configuration")]
 
    public static class ApolloConfigurationManager
    {
        private static readonly Exception? Exception;
        public static IConfigManager? Manager { get; }

        static ApolloConfigurationManager()
        {
            try
            {
                Manager = new DefaultConfigManager(new DefaultConfigRegistry(), new ConfigRepositoryFactory(new ConfigUtil()));
            }
            catch (Exception ex)
            {
                Exception = ex;
            }
        }

        /// <summary>
        /// Get Application's config instance. </summary>
        /// <returns> config instance </returns>
        public static Task<IConfig> GetAppConfig() => GetConfig(ConfigConsts.NamespaceApplication);

        /// <summary>
        /// Get the config instance for the namespace. </summary>
        /// <param name="namespaceName"> the namespace of the config </param>
        /// <returns> config instance </returns>
        public static Task<IConfig> GetConfig(string namespaceName)
        {
            if (string.IsNullOrEmpty(namespaceName)) throw new ArgumentNullException(nameof(namespaceName));

            if (Exception != null) throw new InvalidOperationException("Apollo初始化异常", Exception);

            return Manager!.GetConfig(namespaceName);
        }

        /// <summary>
        /// Get the config instance for the namespace. </summary>
        /// <param name="namespaces"> the namespaces of the config, order desc. </param>
        /// <returns> config instance </returns>
        public static Task<IConfig> GetConfig(params string[] namespaces) => GetConfig((IEnumerable<string>)namespaces);

        /// <summary>
        /// Get the config instance for the namespace. </summary>
        /// <param name="namespaces"> the namespaces of the config, order desc. </param>
        /// <returns> config instance </returns>
        public static async Task<IConfig> GetConfig(IEnumerable<string> namespaces)
        {
            if (namespaces == null) throw new ArgumentNullException(nameof(namespaces));
 
            return new MultiConfig(await Task.WhenAll(namespaces.Reverse().Distinct().Select(GetConfig)).ConfigureAwait(false));
 
        }
    }
}

