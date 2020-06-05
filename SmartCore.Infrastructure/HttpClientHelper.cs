using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SmartCore.Infrastructure
{
    /// <summary>
    /// 
    /// </summary>
    public class HttpClientHelper
    {
        /// <summary>
        /// 
        /// </summary>
        private readonly IHttpClientFactory _clientFactory;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="clientFactory"></param>
        public HttpClientHelper(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <param name="method"></param>
        /// <param name="reqParams"></param>
        /// <returns></returns>
        public async Task<string> SendAsyncCore(string url, HttpMethod method, string reqParams = "")
        {
            using (HttpRequestMessage reqMessage = new HttpRequestMessage(method, url))
            {
                reqMessage.Content = new StringContent(reqParams, Encoding.UTF8, "application/json");

                using (var resMessage = await _clientFactory.CreateClient("kmauth").SendAsync(reqMessage))
                {
                    resMessage.EnsureSuccessStatusCode();
                    return await resMessage.Content.ReadAsStringAsync().ConfigureAwait(false);
                }
            }
        }
    }
}
