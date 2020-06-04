using System;
using System.Collections.Generic;
using System.Text;

namespace SmartCore.Infrastructure.Mongodb
{
    /// <summary>
    /// 
    /// </summary>
    public class MongodbConfig
    {
        /// <summary>
        /// 连接字符串
        /// </summary>
        public string Connection { get; set; }
        /// <summary>
        /// 库
        /// </summary>
        public string DataBase { get; set; }
        /// <summary>
        /// 表
        /// </summary>
        public string Table { get; set; }

    }
}
