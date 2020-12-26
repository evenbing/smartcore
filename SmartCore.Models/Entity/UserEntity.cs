using System;
using Dapper.Contrib.Extensions;

namespace SmartCore.Models.Entity
{
    [Table(TableName)]
    public class UserAccountEntity
    {
        public const string TableName = "UserAccount";
        /// <summary>
        /// 
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 
        /// </summary>

        public string UserEmail { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string NickName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string RealName { get; set; }
    }
}
