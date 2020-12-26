using System;
using System.Collections.Generic;
using System.Text;

namespace SmartCore.Models.DTO
{
    public class UserLoginDTO
    {
        /// <summary>
        /// 应用id
        /// </summary>
        public string AppId { get; set; }
        /// <summary>
        /// 跳转地址
        /// </summary>
        public string RedirectUrl { get; set; }
        /// <summary>
        /// 租户id 针对多租户的情况
        /// </summary>
        public int TenantId { get; set; }
        /// <summary>
        /// 用户名
        /// </summary>
        public string Username { get; set; }
        /// <summary>
        /// 用户密码
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// 登录来源 
        /// </summary>
        public string Source { get; set; }
        /// <summary>
        /// 登录方式 
        /// </summary>
        public int LoginType { get; set; }
    }
}
