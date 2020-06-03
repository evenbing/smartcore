using System;
using System.Collections.Generic;
using System.Text;

namespace SmartCore.Models.DTO
{
    /// <summary>
    /// 
    /// </summary>
    public class JwtAuthorizationDTO
    {
        /// <summary>
        /// 用户id
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// token类型
        /// </summary>
        public string token_type { get; set; }
        /// <summary>
        /// 登录凭证
        /// </summary>
        public AccessToken access_token { get; set; }
        /// <summary>
        /// 刷新token
        /// </summary>
        public AccessToken refresh_token { get; set; }
    }
    public class AccessToken 
    {
        /// <summary>
        /// token
        /// </summary>
        public string token { get; set; }
        /// <summary>
        /// 凭证有效时间，单位：分钟
        /// </summary>
        public int expires_in { get; set; }
        /// <summary>
        /// 过期时间(时间戳)
        /// </summary>
        public long expired { get; set; }
    }
}
