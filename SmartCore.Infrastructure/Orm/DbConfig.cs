using System;
using System.Collections.Generic;
using System.Text;

namespace SmartCore.Infrastructure.Orm
{
    /// <summary>
    /// 数据库配置信息
    /// </summary>
   public class DbConfig
    {
        /// <summary>
        /// 主库链接字符串
        /// </summary>
        public string AppMasterDbConnection { get; set; }
        /// <summary>
        /// 从库链接字符串
        /// </summary>
        public string AppSlaveDbConnection { get; set; }
        /// <summary>
        /// 数据库类型
        /// </summary>
        public string DatabaseType { get; set; }
    }
}
