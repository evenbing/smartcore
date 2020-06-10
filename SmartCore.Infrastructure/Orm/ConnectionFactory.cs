using SmartCore.Infrastructure.Config;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading;

namespace SmartCore.Infrastructure.Orm
{
    public class ConnectionFactory
    {
        private static DbConfig dbConfig= new DbConfig();
        static ConnectionFactory()
        {
            dbConfig = ConfigUtil.GetAppSettings<DbConfig>("DbConfig"); 
        }
        #region 全局变量
       
        private static ThreadLocal<DataSourceEnum> _dataSourceEnum = new ThreadLocal<DataSourceEnum>();

        #endregion

        #region 属性
        /// <summary>
        /// 主库连接字符串 属性  from 配置文件
        /// </summary>
        public static string ConnectionString
        {
            get { return dbConfig.AppMasterDbConnection; }
        }
        /// <summary>
        /// end库连接字符串 属性  from 配置文件
        /// </summary>
        public static List<string> SlaveDbConnectionString
        {
            get { return dbConfig.AppSlaveDbConnection; }
        }
        public static string CurrentDatabaseType
        {
            get { return dbConfig.DatabaseType; }
        }
        /// <summary>
        /// 当前线程数据源 
        /// </summary>
        /// <param name="sourceEnum"></param>     
        public static DataSourceEnum DataSource
        {
            set { _dataSourceEnum.Value = value; }
            get { return _dataSourceEnum.Value; }
        }
        #endregion

        #region 创建数据库连接
        /// <summary>
        /// 创建数据库连接
        /// </summary> 
        /// <param name="connectionString">连接字符串</param>
        /// <returns>IDbConnection</returns>
        public static IDbConnection CreateConnection(DatabaseTypeEnum dbType,string connectionString)
        {

            try
            {
                IDbConnection conn = null;
                switch (dbType)
                {
                    case DatabaseTypeEnum.SqlServer:
                        conn = new System.Data.SqlClient.SqlConnection(connectionString);
                        break;
                    case DatabaseTypeEnum.MySql:
                        conn = new MySql.Data.MySqlClient.MySqlConnection(connectionString);
                        break;
                    case DatabaseTypeEnum.Oracle:
                        //connection = new Oracle.DataAccess.Client.OracleConnection(strConn);
                        //connection = new System.Data.OracleClient.OracleConnection(strConn);
                        break;
                    case DatabaseTypeEnum.DB2:
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
            var dbType = GetDataBaseType(CurrentDatabaseType);
            return CreateConnection(dbType, ConnectionString);
        }
        #endregion

        #region 打开从库连接字符串
        /// <summary>
        /// 打开从库连接字符串
        /// </summary>
        /// <returns></returns>
        public static IDbConnection OpenSlaveConnection()
        {
            var dbType = GetDataBaseType(CurrentDatabaseType);
            int slaveDbConnectionCount = SlaveDbConnectionString.Count;
            if (slaveDbConnectionCount > 0)
            {
                Random random = new Random();
                int index = random.Next(0, slaveDbConnectionCount-1);
                return CreateConnection(dbType, SlaveDbConnectionString[index]);
            }
            else
            { 
                return OpenConnection();
            } 
        }
        #endregion

        #region 私有方法 转换数据库类型
        /// <summary>
        /// 转换数据库类型
        /// </summary>
        /// <param name="databaseType">数据库类型</param>
        /// <returns></returns>
        private static DatabaseTypeEnum GetDataBaseType(string databaseType)
        {
            DatabaseTypeEnum returnValue = DatabaseTypeEnum.SqlServer;
            foreach (DatabaseTypeEnum dbType in Enum.GetValues(typeof(DatabaseTypeEnum)))
            {
                if (dbType.ToString().Equals(databaseType, StringComparison.OrdinalIgnoreCase))
                {
                    returnValue = dbType;
                    break;
                }
            }
            return returnValue;
        }
        #endregion
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queryOrExecSqlFunc"></param>
        /// <returns></returns>
        private T UseDbConnection<T>(Func<IDbConnection, T> queryOrExecSqlFunc)
        {
            IDbConnection dbConn = null;

            try
            {
                //Type modelType = typeof(T);
                //var typeMap = Dapper.SqlMapper.GetTypeMap(modelType);
                //if (typeMap == null || !(typeMap is ColumnAttributeTypeMapper<T>))
                //{
                //    Dapper.SqlMapper.SetTypeMap(modelType, new ColumnAttributeTypeMapper<T>());
                //} 
                dbConn = OpenConnection();  
                return queryOrExecSqlFunc(dbConn);
            }
            catch(Exception ex)
            {
                throw ex;
            } 
        } 
    }

 

}
