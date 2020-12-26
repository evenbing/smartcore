using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartCore.Middleware.Providers
{
    /// <summary>
    /// 自定义授权策略提供程序
    /// </summary>
    public class CustomAuthorizationPolicyProvider : IAuthorizationPolicyProvider
    {
        /// <summary>
        /// 
        /// </summary>
        private AuthorizationOptions _options;
        public DefaultAuthorizationPolicyProvider FallbackPolicyProvider { get; }

        /// <summary>
        /// 构造函数 依赖注入的方式
        /// </summary>
        /// <param name="options"></param>
        public CustomAuthorizationPolicyProvider(IOptions<AuthorizationOptions> options)
        {
            _options = options.Value;
            FallbackPolicyProvider = new DefaultAuthorizationPolicyProvider(options);
        }
        /// <summary>
        /// 返回默认授权策略（用于未指定策略的 [Authorize] 属性的策略）。
        /// </summary>
        /// <returns></returns>
        public Task<AuthorizationPolicy> GetDefaultPolicyAsync() => FallbackPolicyProvider.GetDefaultPolicyAsync();

        /// <summary>
        /// 当未指定策略时， GetFallbackPolicyAsync将返回后备授权策略（授权中间件所使用的策略）。
        /// </summary>
        /// <returns></returns>

        public Task<AuthorizationPolicy> GetFallbackPolicyAsync() => FallbackPolicyProvider.GetFallbackPolicyAsync();

        /// <summary>
        /// 返回给定名称的授权策略
        /// </summary>
        /// <param name="policyName"></param>
        /// <returns></returns>
        public Task<AuthorizationPolicy> GetPolicyAsync(string policyName)
        {
            //先判断当前AuthorzationOptions里面是否包含policyName
            AuthorizationPolicy policy = _options.GetPolicy(policyName);
            //如果有直接返回
            if (policy != null)
            {
                return Task.FromResult(policy);
            }
            //如果没有，解析policyName，并加入到options
            string[] cliams = policyName.Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
             _options.AddPolicy(policyName, builder =>
            {
                builder.RequireClaim(cliams[0], cliams[1]);
            });
            //return FallbackPolicyProvider.GetPolicyAsync(policyName);
            return Task.FromResult(_options.GetPolicy(policyName));
        }
    }
}
