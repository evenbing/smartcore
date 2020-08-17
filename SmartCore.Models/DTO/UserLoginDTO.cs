using System;
using System.Collections.Generic;
using System.Text;

namespace SmartCore.Models.DTO
{
    /// <summary>
    /// 用户登录
    /// </summary>
   public class UserLoginDTO
    {
        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string UserPwd { get; set; }
        /// <summary>
        /// 登录方式
        /// </summary>
        public int LoginType { get; set; }
    }
}
