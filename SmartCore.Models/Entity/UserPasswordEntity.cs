using Dapper.Contrib.Extensions;
using System;
namespace SmartCore.Models.Entity
{

    [Table(TableName)]
    public class UserPasswordEntity
    {
        public const string TableName = "UserPassword";
        /// <summary>
        /// 主键id
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 用户主键id
        /// </summary>
        public int UserId { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        public string PasswordHash { get; set; }
        /// <summary>
        /// 加盐
        /// </summary>
        public string PasswordSalt { get; set; }
        /// <summary>
        /// 安全Key(guid+10位时间戳) 每次登录重新生成
        /// </summary>
        public string SecurityKey { get; set; }
    }

}
