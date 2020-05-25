using System;
using System.Collections.Generic;
using System.Text;

namespace SmartCore.Services
{
    public class TokenManagement
    {
        //[JsonProperty("secret")]
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
    }
}
