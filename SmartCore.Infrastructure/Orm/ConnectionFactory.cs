using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace SmartCore.Infrastructure.Orm
{
    public class ConnectionFactory
    {
        #region 全局变量
        /// <summary>
        /// 主库数据库连接字符串 from 配置文件
        /// </summary>
        private static readonly string connectionString = "";// Geely.Com.Config.BaseConfigs.AppMasterDbConnection;
        #endregion

        #region 属性
        /// <summary>
        /// 主库连接字符串 属性  from 配置文件
        /// </summary>
        public static string ConnectionString
        {
            get { return ""; }
        }
        /// <summary>
        /// end库连接字符串 属性  from 配置文件
        /// </summary>
        public static string EndDbConnectionString
        {
            get { return ""; }
        }
        #endregion

        #region 创建数据库连接
        /// <summary>
        /// 创建数据库连接
        /// </summary> 
        /// <param name="connectionString">连接字符串</param>
        /// <returns>IDbConnection</returns>
        public static IDbConnection CreateConnection(DatabaseType dbType,string connectionString)
        {

            try
            {
                IDbConnection conn = null;
                switch (dbType)
                {
                    case DatabaseType.SqlServer:
                        conn = new System.Data.SqlClient.SqlConnection(connectionString);
                        break;
                    case DatabaseType.MySql:
                        conn = new MySql.Data.MySqlClient.MySqlConnection(connectionString);
                        break;
                    case DatabaseType.Oracle:
                        //connection = new Oracle.DataAccess.Client.OracleConnection(strConn);
                        //connection = new System.Data.OracleClient.OracleConnection(strConn);
                        break;
                    case DatabaseType.DB2:
                        //conn = new System.Data.OleDb.OleDbConnection(connectionString);
                        break;
                }
                if (conn.State != ConnectionState.Open)
                {
                    conn.Open();
                }
                return conn;
            }
            catch (Exception ex)
            {
                throw new DataException(string.Format("CreateConnection Unable to open connection to {0}.", connectionString), ex);
            }
        }
        #endregion

        #region 打开默认的数据库连接
        /// <summary>
        /// 打开默认的数据库连接
        /// </summary>
        /// <returns></returns>
        public static IDbConnection OpenConnection()
        {
            return CreateConnection(DatabaseType.SqlServer,ConnectionString);
        }
        #endregion
    }

    public enum DatabaseType
    {
        SqlServer,
        MySql,
        Oracle,
        DB2
    }

}
