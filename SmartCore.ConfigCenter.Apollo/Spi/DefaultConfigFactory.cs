using SmartCore.ConfigCenter.Apollo.Internals;
using System.Threading.Tasks;
namespace SmartCore.ConfigCenter.Apollo.Spi
{
    public class DefaultConfigFactory : IConfigFactory
    {
        private readonly IConfigRepositoryFactory _repositoryFactory;

        public DefaultConfigFactory(IConfigRepositoryFactory repositoryFactory) => _repositoryFactory = repositoryFactory;

        public async Task<IConfig> Create(string namespaceName)
        {
            var configRepository = _repositoryFactory.GetConfigRepository(namespaceName);

            var config = new DefaultConfig(namespaceName, configRepository);

            await config.Initialize().ConfigureAwait(false);

            return config;
        }
    }
}
