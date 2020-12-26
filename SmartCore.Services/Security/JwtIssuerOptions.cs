using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SmartCore.Services
{
    public class JwtIssuerOptions
    {
        /// <summary>
        /// 
        /// </summary>
        public string Secret { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Issuer { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Audience { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int AccessExpiration { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int RefreshExpiration { get; set; }
        ///// <summary>
        ///// 
        ///// </summary>
        //public string Subject { get; set; }
        ///// <summary>
        ///// 
        ///// </summary>
        //public DateTime NotBefore => DateTime.UtcNow;
        ///// <summary>
        ///// 
        ///// </summary>
        //public DateTime IssueAt => DateTime.UtcNow;
        ///// <summary>
        ///// 
        ///// </summary>
        //public Func<Task<string>> JtiGenerator => () => Task.FromResult(Guid.NewGuid().ToString());
    }
}
