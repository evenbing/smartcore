using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SmartCore.Services
{
    public class GitHubServices
    {
        private readonly IHttpClientFactory httpClientFactory;
        public GitHubServices(IHttpClientFactory _httpClientFactory)
        {
            httpClientFactory = _httpClientFactory;
        }
        // <summary>
        // Get请求数据
        // <para>最终以url参数的方式提交</para>
        // </summary>
        // <param name="parameters">参数字典,可为空</param>
        // <param name="requestUri">例如/api/Files/UploadFile</param>
        // <returns></returns>
        public async Task<string> Get(Dictionary<string, string> parameters, string requestUri, string token)
        {
            //从工厂获取请求对象   声明自己创建哪一个httpClient客户端
            var client = httpClientFactory.CreateClient("github");
            //添加请求头
            if (!string.IsNullOrWhiteSpace(token))
            {
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
            }
            client.DefaultRequestHeaders.Add("Accept", "application/vnd.github.v3+json");
            client.DefaultRequestHeaders.Add("Content-Type", "application/json; charset=utf-8");
            //拼接地址
            if (parameters != null)
            {
                var strParam = string.Join("&", parameters.Select(o => o.Key + "=" + o.Value));
                requestUri = string.Concat(requestUri, '?', strParam);
            }
            client.BaseAddress = new Uri(requestUri);
            return client.GetStringAsync(requestUri).Result;
            //var response = await client.SendAsync(request);

            //if (response.IsSuccessStatusCode)
            //{
            //    Branches = await response.Content
            //        .ReadAsAsync<IEnumerable<GitHubBranch>>();
            //}
            //else
            //{
            //    GetBranchesError = true;
            //    Branches = Array.Empty<GitHubBranch>();
            //}
        }
    }
}
