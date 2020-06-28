using SmartCore.ConfigCenter.Apollo.Core;
using SmartCore.ConfigCenter.Apollo.Internals;
using SmartCore.ConfigCenter.Apollo.Spi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SmartCore.ConfigCenter.Apollo
{
    /// <summary>
    /// Entry point for client config use
    /// </summary>
    [Obsolete("不建议使用，推荐使用Microsoft.Extensions.Configuration.IConfiguration")]
    public class ApolloConfigurationManager
    {
        private static IConfigManager? _manager;

        public static IConfigManager Manager => _manager ?? throw new InvalidOperationException("请在使用之前调用AddApollo");

        internal static void SetApolloOptions(ConfigRepositoryFactory factory) =>
            Interlocked.CompareExchange(ref _manager, new DefaultConfigManager(new DefaultConfigRegistry(), factory), null);

        /// <summary>
        /// Get Application's config instance. </summary>
        /// <returns> config instance </returns>
        public Task<IConfig> GetAppConfig() => GetConfig(ConfigConsts.NamespaceApplication);

        /// <summary>
        /// Get the config instance for the namespace. </summary>
        /// <param name="namespaceName"> the namespace of the config </param>
        /// <returns> config instance </returns>
        public Task<IConfig> GetConfig(string namespaceName)
        {
            if (string.IsNullOrEmpty(namespaceName)) throw new ArgumentNullException(nameof(namespaceName));
            if (_manager == null) throw new InvalidOperationException("请先配置Apollo");

            return _manager.GetConfig(namespaceName);
        }

        /// <summary>
        /// Get the config instance for the namespace. </summary>
        /// <param name="namespaces"> the namespaces of the config, order desc. </param>
        /// <returns> config instance </returns>
        public Task<IConfig> GetConfig(params string[] namespaces) => GetConfig((IEnumerable<string>)namespaces);

        /// <summary>
        /// Get the config instance for the namespace. </summary>
        /// <param name="namespaces"> the namespaces of the config, order desc. </param>
        /// <returns> config instance </returns>
        public async Task<IConfig> GetConfig(IEnumerable<string> namespaces)
        {
            if (namespaces == null) throw new ArgumentNullException(nameof(namespaces));

            return new MultiConfig(await Task.WhenAll(namespaces.Reverse().Distinct().Select(GetConfig)).ConfigureAwait(false));
        }
    }
}

