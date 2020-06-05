﻿using System;
using Dapper.Contrib.Extensions;

namespace SmartCore.Models.Entity
{
    [Table(TableName)]
    public class UserEntity
    {
        public const string TableName = "User";
        /// <summary>
        /// 
        /// </summary>
        public int Id { get; set; }

        public string UserName { get; set; }

        public string UserEmail { get; set; }

        public string NickName { get; set; }
        public string RealName { get; set; }
    }
}
