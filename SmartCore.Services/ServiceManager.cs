using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SmartCore.Services
{
    /// <summary>
    /// 策略委托
    /// </summary>
    /// <param name="serviceName"></param>
    /// <param name="services"></param>
    /// <param name="hashKey"></param>
    /// <returns></returns>
    public delegate ServiceEntry[] StrategyDelegate(string serviceName, ServiceEntry[] services, string hashKey = null);

    public class ServiceManager : IServiceManager
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger _logger;
        private readonly IConsulClient _consulClient;
        private readonly IList<Func<StrategyDelegate, StrategyDelegate>> _components;
        private StrategyDelegate _strategy;

        public ServiceManager(IHttpClientFactory httpClientFactory,
            IConsulClient consulClient,
            ILogger<ServiceManager> logger)
        {
            this._cancellationTokenSource = new CancellationTokenSource();
            this._components = new List<Func<StrategyDelegate, StrategyDelegate>>();

            this._httpClientFactory = httpClientFactory;
            this._optionsConsulConfig = optionsConsulConfig;
            this._logger = logger;
            this._consulClient = consulClient;
        }

        public async Task<HttpClient> GetHttpClientAsync(string serviceName, string errorIPAddress = null, string hashkey = null)
        {
            //重要：获取所有健康的服务
            var resonse = (await this._consulClient.Health.Service(serviceName.ToLower(), this._cancellationTokenSource.Token)).Response;
            var filteredService = this.GetServiceNode(serviceName, resonse.ToArray(), hashkey);
            return this.CreateHttpClient(serviceName.ToLower(), filteredService.Service.Address, filteredService.Service.Port);
        }

        private ServiceEntry GetServiceNode(string serviceName, ServiceEntry[] services, string hashKey = null)
        {
            if (this._strategy == null)
            {
                lock (this) { if (this._strategy == null) this._strategy = this.Build(); }
            }

            //策略过滤
            var filterService = this._strategy(serviceName, services, hashKey);
            return filterService.FirstOrDefault();
        }

        private HttpClient CreateHttpClient(string serviceName, string address, int port)
        {
            var httpClient = this._httpClientFactory.CreateClient(serviceName);
            httpClient.BaseAddress = new System.Uri($"http://{address}:{port}");
            return httpClient;
        }
    }
}
