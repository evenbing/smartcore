using System;
using System.Collections.Generic;
using System.Text;

namespace SmartCore.Models.DTO
{
    /// <summary>
    /// 
    /// </summary>
    public class UserTokenDTO
    {
        /// <summary>
        /// 用户id
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 用户密码
        /// </summary>
        public string UserPass { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string UserRole { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Email { get; set; }
       
    }
}
