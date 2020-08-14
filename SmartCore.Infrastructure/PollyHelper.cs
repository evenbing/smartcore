using Microsoft.Extensions.Logging;
using Polly;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartCore.Infrastructure
{
    /// <summary>
    /// 
    /// </summary>
    /// <example>
    /// _pollyHelper.PollyRetry<UserServicesDomainException>(() =>
    ///       {
    ///           throw new UserServicesDomainException("调用服务出现异常");
    ///});
    /// </example>
    public class PollyHelper
    {
        private ILogger<PollyHelper> _logger;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        public PollyHelper(ILogger<PollyHelper> logger)
        {
            this._logger = logger;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="exception"></param>
        /// <param name="action"></param>
        public void PollyRetry<T>(Action action) where T : Exception
        {
            var policy = Policy
               .Handle<T>()
               .Retry(2, (ex, count) =>
               {
                   _logger.LogError("执行失败!重试次数 {0}\r\n异常来自 {1}", count, ex.GetType().Name);
               });

            policy.Execute(action);
        }
    }
}
