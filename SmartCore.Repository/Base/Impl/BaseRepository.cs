using Dapper;
using SmartCore.Infrastructure.Orm;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper.Contrib.Extensions;
using System.Data;
using System.Linq.Expressions;
using System.Reflection.Metadata;
using SmartCore.Repository.Sys.Impl;
using System.Threading;

namespace SmartCore.Repository.Base.Impl
{
    public class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : class, new()
    {
        #region 全局变量
        /// <summary>
        /// 列名缓存
        /// </summary>
        private static readonly ConcurrentDictionary<string, string> ColumnsCache = new ConcurrentDictionary<string, string>();
        private static ThreadLocal<DataSourceEnum> _dataSourceEnum = new ThreadLocal<DataSourceEnum>();
        #endregion

        #region 属性

        /// <summary>
        /// 私有变量 表名
        /// </summary>
        private string _tableName = "";
        /// <summary>
        /// 表名
        /// </summary>
        public string TableName
        {
            get
            {
                if (!string.IsNullOrEmpty(_tableName))
                {
                    return _tableName;
                }
                else
                {

                    var attrList = System.Attribute.GetCustomAttributes(typeof(TEntity));
                    if (attrList != null && attrList.Count() > 0)
                    {
                        System.Attribute attributeFirst = attrList.Where(p => (p.TypeId as dynamic).Name.Equals("TableAttribute")).FirstOrDefault();
                        return (attributeFirst as dynamic).Name;
                    }
                    string typeName = typeof(TEntity).Name;
                    if (typeName.EndsWith("Entity"))
                    {
                        typeName = typeName.Substring(0, typeName.Length - 6);
                    }
                    if (typeName.EndsWith("Dto"))
                    {
                        typeName = typeName.Substring(0, typeName.Length - 3);
                    }
                    return typeName;
                }
            }
            set { _tableName = value; }
        }

        ///// <summary>
        ///// 当前线程数据源 
        ///// </summary>
        ///// <param name="sourceEnum"></param>     
        //public  DataSourceEnum DataSource
        //{
        //    set { _dataSourceEnum.Value = value; }
        //    get { return _dataSourceEnum.Value; }
        //}
        #endregion

        #region 根据表名获取列名
        /// <summary>
        /// 根据表名获取列名
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <returns></returns>
        public async Task<List<string>> GetColumnNameList(string tableName)
        {
            string sql = "";
            string[] tableSchemaAndTable = tableName.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
            if (tableSchemaAndTable.Length > 1)
            {
                sql = "SELECT COLUMN_NAME  FROM information_schema.columns where TABLE_SCHEMA=@TableSchema AND TABLE_NAME=@TableName";
                using (var dbConnection = ConnectionFactory.OpenConnection())
                {
                    var result = await dbConnection.QueryAsync<string>(sql, new { TableSchema = tableSchemaAndTable[0], TableName = tableSchemaAndTable[1] });
                    return result.AsList();
                }
            }
            else
            {
                sql = "SELECT COLUMN_NAME  FROM information_schema.columns where  TABLE_NAME=@TableName";
                using (var dbConnection = ConnectionFactory.OpenConnection())
                {
                    var result = await dbConnection.QueryAsync<string>(sql, new { TableName = tableName });
                    return result.ToList();
                }
            }
        }
        #endregion

        #region 私有方法
        /// <summary>
        /// 获取默认值
        /// </summary> 
        /// <returns></returns>
        private TEntity GetDefault()
        {
            TEntity value;
            var type = typeof(TEntity);
            if (type.IsClass && type != typeof(string))
            {
                value = Activator.CreateInstance<TEntity>();
            }
            else
            {
                value = default(TEntity);
            }
            return value;
        }
        /// <summary>
        /// 获取列名
        /// </summary>
        /// <returns></returns>
        private string GetColumns()
        {
            List<string> result = GetColumnNameList(TableName).Result;
            if (result != null)
            {
                return string.Join(",", result);
            }
            return "*";
            //var type = typeof(T);
            //PropertyInfo[] properties = type.GetProperties();
            //StringBuilder builder = new StringBuilder();
            //if (properties != null && properties.Length > 0)
            //{
            //    //排查未映射的字段
            //    var propList = properties.Where(p => !Attribute.IsDefined(p, typeof(WriteAttribute))).ToList();
            //    if (propList != null && propList.Count > 0)
            //    {
            //        propList.ForEach(p =>
            //        {
            //            builder.AppendFormat("{0},", p.Name);
            //        });
            //        if (builder.Length > 0)
            //        {
            //            return builder.ToString().TrimEnd(',');
            //        }
            //    }
            //}
            //return " * ";
        }
        private string GetColumnsByCache()
        {
            string key = "BPM:column:" + TableName + "_" + typeof(TEntity).GetHashCode();
            if (!ColumnsCache.ContainsKey(key))
            {
                ColumnsCache[key] = GetColumns();
            }
            return ColumnsCache[key];
        }
        #endregion

        #region 新增
        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<int> Add(TEntity entity)
        {
            using (var dbConnection = ConnectionFactory.OpenConnection())
            {
                var result = await dbConnection.InsertAsync(entity);
                return result;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        public async Task<int> BatchAdd(List<TEntity> entities)
        {
            using (var dbConnection = ConnectionFactory.OpenConnection())
            {
                var result = await dbConnection.InsertAsync(entities);
                return result;
            }
        }
        #endregion

        #region 修改
        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<bool> Update(TEntity entity)
        {
            using (var dbConnection = ConnectionFactory.OpenConnection())
            {
                var result = await dbConnection.UpdateAsync(entity);
                return result;
            }
        }
        //public async Task<bool> Update(TEntity entity, params Expression<Func<TEntity, object>>[] properties)
        //{
        //    using (var dbConnection = ConnectionFactory.OpenConnection())
        //    {
        //        foreach (var property in properties)
        //        {
        //            string propertyName = "";
        //            Expression bodyExpression = property.Body;
        //            if (bodyExpression.NodeType == ExpressionType.Convert && bodyExpression is UnaryExpression)
        //            {
        //                Expression operand = ((UnaryExpression)property.Body).Operand;
        //                propertyName = ((MemberExpression)operand).Member.Name;
        //            }
        //            else
        //            {
        //                propertyName = ExpressionHelper.GetExpressionText(property);
        //            } 
        //        }
        //        var result = await dbConnection.UpdateAsync(entity);
        //        return result;
        //    }
        //}
        /// <summary>
        /// 批量更新实体数据
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        public async Task<bool> Update(List<TEntity> entities)
        {
            using (var dbConnection = ConnectionFactory.OpenConnection())
            {
                var result = await dbConnection.UpdateAsync(entities);
                return result;
            }
        }

        /// <summary>
        /// 根据sql语句来更新
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="paramters"></param>
        /// <returns></returns>
        public async Task<bool> UpdateBySql(string sql, object paramters = null, CommandType commandType = CommandType.Text)
        {
            using (var dbConnection = ConnectionFactory.OpenConnection())
            {
                bool result = await dbConnection.ExecuteAsync(sql, paramters, null, null, commandType) > 0;
                return result;
            }
        }
        /// <summary>
        /// 根据sql语句来更新数据
        /// </summary>
        /// <param name="dbConnection">数据库连接对象</param>
        /// <param name="sql">sql语句</param>
        /// <param name="paramters">动态参数</param>
        /// <param name="transaction">数据库事务对象 可空</param>
        /// <returns></returns>
        public async Task<bool> UpdateBySql(IDbConnection dbConnection, string sql, object paramters = null, IDbTransaction transaction = null)
        {
            bool result = await dbConnection.ExecuteAsync(sql, paramters, transaction) > 0;
            return result;
        }
        /// <summary>
        /// 字段及对应的值
        /// </summary>
        /// <param name="keyValuePairs"></param>
        /// <returns></returns>
        public async Task<bool> UpdatePartialFields(Dictionary<string, object> keyValuePairs)
        {
            StringBuilder sqlBuilder = new StringBuilder();
            DynamicParameters parameters = new DynamicParameters();
            if (keyValuePairs != null && keyValuePairs.Count > 0)
            {
                sqlBuilder.AppendFormat(" UPDATE {0} SET ", TableName);
                int i = 0;
                foreach (var item in keyValuePairs)
                {
                    if (item.Key.Equals("Id"))
                    {
                        continue;
                    }
                    i++;
                    if (i == 1)
                    {
                        sqlBuilder.AppendFormat(" {0}=@{0}", item.Key);
                    }
                    else
                    {
                        sqlBuilder.AppendFormat(",{0}=@{0}", item.Key);
                    }
                    parameters.Add(string.Format("@{0}", item.Key), item.Value);
                }
                sqlBuilder.Append(" WHERE Id=@Id");
                parameters.Add("@Id", keyValuePairs["Id"]);
                using (var dbConnection = ConnectionFactory.OpenConnection())
                {
                    bool result = await dbConnection.ExecuteAsync(sqlBuilder.ToString(), parameters) > 0;
                }
            }
            return false;
        }
        #endregion

        #region 删除
        /// <summary>
        /// 删除当前实体的表数据
        /// </summary>
        /// <returns></returns>
        public async Task<bool> Delete(TEntity entity)
        {
            using (var dbConnection = ConnectionFactory.OpenConnection())
            {
                var result = await dbConnection.DeleteAsync(entity);
                return result;
            }
        }
        /// <summary>
        /// 根据主键id删除的表数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<bool> Delete(dynamic id)
        {
            using (var dbConnection = ConnectionFactory.OpenConnection())
            {
                var sql = string.Format("DELETE FROM {0} WHERE Id=@Id", TableName);
                var result = await dbConnection.ExecuteAsync(sql, new { Id = id });
                return result > 0;
            }
        }
        /// <summary>
        /// 删除多个主键id的表数据
        /// </summary>
        /// <param name="idList">主键id列表</param>
        /// <returns></returns>
        public async Task<bool> Delete(List<int> idList)
        {
            using (var dbConnection = ConnectionFactory.OpenConnection())
            {
                var sql = string.Format("DELETE FROM {0} WHERE Id=@IdArray", TableName);
                var result = await dbConnection.ExecuteAsync(sql, new { IdArray = idList.ToArray() });
                return result > 0;
            }
        }
        /// <summary>
        /// 根据多个主键id批量删除数据
        /// </summary>
        /// <param name="dbConnection"></param>
        /// <param name="idList"></param>
        /// <param name="transaction">数据库事务对象</param>
        /// <returns></returns>
        public async Task<bool> Delete(IDbConnection dbConnection, List<int> idList, IDbTransaction transaction = null)
        {
            var sql = string.Format("DELETE FROM {0} WHERE Id IN @Ids", TableName);
            bool result = await dbConnection.ExecuteAsync(sql, new { Ids = idList.ToArray() }, transaction) > 0;
            return result;
        }
        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        public async Task<bool> Delete(List<TEntity> entities)
        {
            using (var dbConnection = ConnectionFactory.OpenConnection())
            {
                var result = await dbConnection.DeleteAsync(entities);
                return result;
            }
        }
        #endregion

        #region 查询

        #region 获取单条实体信息
        /// <summary>
        /// 根据主键id获取单条实体信息
        /// </summary>
        /// <param name="id">主键id</param>
        /// <returns></returns>
        public async Task<TEntity> Get(int id, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            using (var dbConnection = ConnectionFactory.OpenConnection())
            {
                return await dbConnection.GetAsync<TEntity>(id, transaction, commandTimeout);
            }
        }
        /// <summary>
        /// 根据主键id获取单条实体信息
        /// </summary>
        /// <param name="id"></param>
        /// <param name="transaction">数据库事务对象</param>
        /// <param name="commandTimeout">命令执行超时时间</param>
        /// <returns></returns>
        public async Task<TEntity> Get(long id, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            using (var dbConnection = ConnectionFactory.OpenConnection())
            {
                return await dbConnection.GetAsync<TEntity>(id, transaction, commandTimeout);
            }
        }
        /// <summary>
        /// 根据主键id获取单条实体信息
        /// </summary>
        /// <param name="id"></param>
        /// <param name="transaction">数据库事务对象</param>
        /// <param name="commandTimeout">命令执行超时时间</param>
        /// <returns></returns>
        public async Task<TEntity> Get(string id, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            using (var dbConnection = ConnectionFactory.OpenConnection())
            {
                return await dbConnection.GetAsync<TEntity>(id, transaction, commandTimeout);
            }
        }
        /// <summary>
        /// 根据sql获取当前实体类
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="parameters">动态参数</param>
        /// <returns></returns>
        public async Task<TEntity> Get(string sql, object parameters = null, CommandType commandType = CommandType.Text)
        {
            using (var dbConnection = ConnectionFactory.OpenConnection())
            {
                return await dbConnection.QueryFirstOrDefaultAsync<TEntity>(sql, parameters, null, null, commandType);
            }
        }
        /// <summary>
        /// 根据sql获取当前实体类
        /// </summary>
        /// <param name="dbConnection">连接字符串</param>
        /// <param name="sql">sql语句</param>
        /// <param name="parameters">动态参数</param>
        /// <returns></returns>
        public async Task<TEntity> Get(IDbConnection dbConnection, string sql, object parameters = null, CommandType commandType = CommandType.Text)
        {
            return await dbConnection.QueryFirstOrDefaultAsync<TEntity>(sql, parameters, null, null, commandType);
        }
        /// <summary>
        /// 根据sql语句获取泛型实体类
        /// </summary>
        /// <typeparam name="T">泛型类</typeparam>
        /// <param name="sql">sql语句</param>
        /// <param name="parameters">动态参数</param>
        /// <returns></returns>
        public async Task<T> Get<T>(string sql, object parameters = null, CommandType commandType = CommandType.Text)
        {
            using (var dbConnection = ConnectionFactory.OpenConnection())
            {
                return await dbConnection.QueryFirstOrDefaultAsync<T>(sql, parameters, null, null, commandType);
            }
        }
        /// <summary>
        /// 根据sql获取泛型实体类
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        ///  <param name="dbConnection">数据库连接对象</param>
        /// <param name="sql">sql语句</param>
        /// <param name="parameters">动态参数</param>
        /// <returns></returns>
        public async Task<T> Get<T>(IDbConnection dbConnection, string sql, object parameters = null, CommandType commandType = CommandType.Text)
        {
            return await dbConnection.QueryFirstOrDefaultAsync<T>(sql, parameters, null, null, commandType);
        }
        #endregion

        #region 根据id获取实体详细数据
        /// <summary>
        /// 根据id获取实体详细数据 返回数据字典
        /// </summary>
        /// <param name="id">主键id</param>
        /// <returns></returns>
        public async Task<ConcurrentDictionary<string, object>> GetDictionary(dynamic id)
        {
            ConcurrentDictionary<string, object> keyValuePairs = new ConcurrentDictionary<string, object>();
            string sql = $"SELECT {GetColumnsByCache()} FROM {TableName} WHERE Id=@Id";
            using (var dbConnection = ConnectionFactory.OpenConnection())
            {
                using (var reader = await dbConnection.ExecuteReaderAsync(sql, new { Id = id }))
                {
                    if (reader.Read())
                    {
                        for (var i = 0; i < reader.FieldCount; i++)
                        {
                            keyValuePairs[reader.GetName(i)] = reader.GetValue(i);
                        }
                    }
                }
            }
            return keyValuePairs;
        }
        #endregion

        #region 查询数据列表
        /// <summary>
        /// 
        /// </summary>
        /// <param name="transaction">数据库事务对象</param>
        /// <param name="commandTimeout">命令执行超时时间</param>
        /// <returns></returns>
        public async Task<List<TEntity>> QueryAllList(IDbTransaction transaction = null, int? commandTimeout = null)
        {
           // Type chindType = this.GetType();//获取子类的类型 
           //var childAttr = chindType.GetCustomAttributes(typeof(DatatSourceSlaveAttribute), false).FirstOrDefault();
           // if (childAttr != null)
           // {
           //     DataSource = DataSourceEnum.SLAVE;
           // }
           // else
           // {
           //     DataSource = DataSourceEnum.MASTER;
           // }
            using (var dbConnection = ConnectionFactory.OpenConnection())
            {
                var result = await dbConnection.GetAllAsync<TEntity>(transaction, commandTimeout);
                return result.AsList();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="whereExpression">linq条件表达式</param>
        /// <param name="transaction">数据库事务对象</param>
        /// <param name="commandTimeout">命令执行超时时间</param>
        /// <returns></returns>
        public async Task<List<TEntity>> QueryList(Expression<Func<TEntity, bool>> whereExpression, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            using (var dbConnection = ConnectionFactory.OpenConnection())
            {
                var result = await dbConnection.GetAllAsync<TEntity>(transaction, commandTimeout);
                return result.AsQueryable().Where(whereExpression).ToList();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sql">sql语句或者存储过程名称 存储过程需要指定CommandType=CommandType.StoredProcedure</param>
        /// <param name="parameters">动态参数</param>
        /// <returns></returns>
        public async Task<List<TEntity>> QueryList(string sql, object parameters = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType commandType = CommandType.Text)
        {
            using (var dbConnection = ConnectionFactory.OpenConnection())
            {
                var result = await dbConnection.QueryAsync<TEntity>(sql, parameters, transaction, commandTimeout, commandType);
                return result.ToList();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dbConnection">数据库连接对象</param>
        /// <param name="sql">sql语句或者存储过程名称 存储过程需要指定CommandType=CommandType.StoredProcedure</param>
        /// <param name="parameters">动态参数</param>
        /// <param name="transaction">数据库事务对象</param>
        /// <param name="commandTimeout">命令执行超时时间</param>
        /// <param name="commandType">执行命令类型 text和存储过程</param>
        /// <returns></returns>
        public async Task<List<TEntity>> QueryList(IDbConnection dbConnection, string sql, object parameters = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType commandType = CommandType.Text)
        {
            var result = await dbConnection.QueryAsync<TEntity>(sql, parameters, transaction, commandTimeout, commandType);
            return result.ToList();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sql">sql语句或者存储过程名称 存储过程需要指定CommandType=CommandType.StoredProcedure</param>
        /// <param name="parameters">动态参数</param>
        /// <param name="transaction">数据库事务对象</param>
        /// <param name="commandTimeout">命令执行超时时间</param>
        /// <param name="commandType">执行命令类型 text和存储过程</param>
        /// <returns></returns>
        public async Task<List<T>> QueryList<T>(string sql, object parameters = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType commandType = CommandType.Text)
        {
            using (var dbConnection = ConnectionFactory.OpenConnection())
            {
                var result = await dbConnection.QueryAsync<T>(sql, parameters, transaction, commandTimeout, commandType);
                return result.ToList();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dbConnection">数据库连接对象</param>
        /// <param name="sql">sql语句或者存储过程名称 存储过程需要指定CommandType=CommandType.StoredProcedure</param>
        /// <param name="parameters">动态参数</param>
        /// <param name="transaction">数据库事务对象</param>
        /// <param name="commandTimeout">命令执行超时时间</param>
        /// <param name="commandType">执行命令类型 text和存储过程</param>
        /// <returns></returns>
        public async Task<List<T>> QueryList<T>(IDbConnection dbConnection, string sql, object parameters = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType commandType = CommandType.Text)
        {
            var result = await dbConnection.QueryAsync<T>(sql, parameters, transaction, commandTimeout, commandType);
            return result.ToList();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sql">sql语句或者存储过程名称 存储过程需要指定CommandType=CommandType.StoredProcedure</param>
        /// <param name="parameters">动态参数</param>
        /// <param name="transaction">数据库事务对象</param>
        /// <param name="commandTimeout">命令执行超时时间</param>
        /// <param name="commandType">执行命令类型 text和存储过程</param>
        /// <returns></returns>
        public async Task<ConcurrentBag<TEntity>> QueryConcurrentBag(string sql, object parameters = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType commandType = CommandType.Text)
        { 
            using (var dbConnection = ConnectionFactory.OpenConnection())
            {
                var result = await dbConnection.QueryAsync<TEntity>(sql, parameters, transaction, commandTimeout, commandType);
                ConcurrentBag<TEntity> list = new ConcurrentBag<TEntity>(result);
                return list;
            } 
        }
        #endregion

        #region 查询分页列表
        #endregion

        #region 获取第一行第一列的数据
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">sql语句或者存储过程名称 存储过程需要指定CommandType=CommandType.StoredProcedure</param>
        /// <param name="parameters">动态参数</param> 
        /// <param name="dbTransaction">数据库事务对象</param>
        /// <param name="commandTimeout">命令执行超时时间</param>
        /// <param name="commandType">执行命令类型 text和存储过程</param>
        /// <returns></returns>
        public async Task<T> ExecuteScalar<T>(string sql, object parameters = null, IDbTransaction dbTransaction = null,
         int? commandTimeout = null, CommandType? commandType = null)
        {
            using (var dbConnection = ConnectionFactory.OpenConnection())
            {
                var result = await dbConnection.ExecuteScalarAsync<T>(sql, parameters, dbTransaction, commandTimeout, commandType);
                return result;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbConnection">数据库连接对象</param>
        /// <param name="sql">sql语句或者存储过程名称 存储过程需要指定CommandType=CommandType.StoredProcedure</param>
        /// <param name="parameters">动态参数</param>
        /// <param name="dbTransaction">数据库事务对象</param>
        /// <param name="commandTimeout">命令执行超时时间</param>
        /// <param name="commandType">执行命令类型 text和存储过程</param>
        /// <returns></returns>
        public async Task<T> ExecuteScalar<T>(IDbConnection dbConnection, string sql, object parameters = null, IDbTransaction dbTransaction = null,
     int? commandTimeout = null, CommandType? commandType = null)
        {
            var result = await dbConnection.ExecuteScalarAsync<T>(sql, parameters, dbTransaction, commandTimeout, commandType);
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="parameters">动态参数</param>
        /// <param name="dbTransaction">数据库事务对象</param>
        /// <param name="commandTimeout">命令执行超时时间</param> 
        /// <param name="commandType">执行命令类型 text和存储过程</param>
        /// <returns></returns>
        public async Task<string> ExecuteScalarToStr(string sql, object parameters = null, IDbTransaction dbTransaction = null, int? commandTimeout = null, CommandType commandType = CommandType.Text)
        {
            using (var dbConnection = ConnectionFactory.OpenConnection())
            {
                var result = await dbConnection.ExecuteScalarAsync<string>(sql, parameters, dbTransaction, commandTimeout, commandType);
                return result;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dbConnection">数据库连接对象</param>
        /// <param name="sql">sql语句或者存储过程名称 存储过程需要指定CommandType=CommandType.StoredProcedure</param>
        /// <param name="parameters">动态参数</param>
        /// <param name="dbTransaction">数据库事务对象</param>
        /// <param name="commandTimeout">命令执行超时时间</param>
        /// <param name="command">执行命令类型 text和存储过程</param>
        /// <returns></returns>
        public async Task<string> ExecuteScalarToStr(IDbConnection dbConnection, string sql, object parameters = null, IDbTransaction dbTransaction = null, int? commandTimeout = null, CommandType commandType = CommandType.Text)
        {
            var result = await dbConnection.ExecuteScalarAsync<string>(sql, parameters, dbTransaction, commandTimeout, commandType);
            return result;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sql">sql语句或者存储过程名称 存储过程需要指定CommandType=CommandType.StoredProcedure</param>
        /// <param name="parameters">动态参数</param>
        /// <param name="dbTransaction">数据库事务对象</param>
        /// <param name="commandTimeout">命令执行超时时间</param>
        /// <param name="command">执行命令类型 text和存储过程</param>
        /// <returns></returns>
        public async Task<int> ExecuteScalarToInt(string sql, object parameters = null, IDbTransaction dbTransaction = null, int? commandTimeout = null, CommandType commandType = CommandType.Text)
        {
            using (var dbConnection = ConnectionFactory.OpenConnection())
            {
                var result = await dbConnection.ExecuteScalarAsync<int>(sql, parameters, dbTransaction, commandTimeout, commandType);
                return result;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="conn">数据库连接对象</param>
        /// <param name="sql">sql语句或者存储过程名称 存储过程需要指定CommandType=CommandType.StoredProcedure</param>
        /// <param name="parameters">动态参数</param>
        /// <param name="dbTransaction">数据库事务对象</param>
        /// <param name="commandTimeout">命令执行超时时间</param>
        /// <param name="commandType">执行命令类型 text和存储过程</param>
        /// <returns></returns>
        public async Task<int> ExecuteScalarToInt(IDbConnection dbConnection, string sql, object parameters = null, IDbTransaction dbTransaction = null, int? commandTimeout = null, CommandType commandType = CommandType.Text)
        {
            var result = await dbConnection.ExecuteScalarAsync<int>(sql, parameters, dbTransaction, commandTimeout, commandType);
            return result;
        }

        #endregion

        #region 获取Datable
        /// <summary>
        /// 获取Datable
        /// </summary>
        /// <param name="sql">sql语句或者存储过程名称 存储过程需要指定CommandType=CommandType.StoredProcedure</param>
        /// <param name="parameters">动态参数</param>
        /// <param name="transaction">数据库事务对象</param>
        /// <param name="commandTimeout">命令执行超时时间</param>
        /// <param name="commandType">执行命令类型 text和存储过程</param>
        /// <returns></returns>
        public async Task<DataTable> QueryTable(string sql, object parameters = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType commandType = CommandType.Text)
        {
            DataTable table = new DataTable();
            using (var dbConnection = ConnectionFactory.OpenConnection())
            {
                using (var reader = await dbConnection.ExecuteReaderAsync(sql, parameters, transaction, commandTimeout, commandType))
                {
                    table.Load(reader);
                }
            }
            return table;
        }
        /// <summary>
        /// 获取Datable
        /// </summary>
        /// <param name="dbConnection">数据库连接对象</param>
        /// <param name="sql">sql语句或者存储过程名称 存储过程需要指定CommandType=CommandType.StoredProcedure</param>
        /// <param name="parameters">动态参数</param>
        /// <param name="transaction">数据库事务对象</param>
        /// <param name="commandTimeout">命令执行超时时间</param>
        /// <param name="commandType">执行命令类型 text和存储过程</param>
        /// <returns></returns>
        public async Task<DataTable> QueryTable(IDbConnection dbConnection, string sql, object parameters = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType commandType = CommandType.Text)
        {
            DataTable table = new DataTable();
            using (var reader = await dbConnection.ExecuteReaderAsync(sql, parameters, transaction, commandTimeout, commandType))
            {
                table.Load(reader);
            }
            return table;
        }

        #endregion


        #endregion

        #region 执行sql语句
        /// <summary>
        /// 执行sql 返回自增id，一般由于sql语句插入到数据表中
        /// </summary>
        /// <param name="sql">sql语句或者存储过程名称 存储过程需要指定CommandType=CommandType.StoredProcedure</param>
        /// <param name="parameters">动态参数</param>
        /// <param name="commandType">执行命令类型 text和存储过程</param>
        /// <returns>返回自增id</returns>
        public async Task<long> ExcuteAndReturnId(string sql, object parameters = null, CommandType commandType = CommandType.Text)
        {
            if (!string.IsNullOrEmpty(sql))
            {
                using (var dbConnection = ConnectionFactory.OpenConnection())
                {
                    var multi = await dbConnection.QueryMultipleAsync(sql, parameters, null, null, commandType);
                    var first = multi.Read().FirstOrDefault();
                    if (first == null || first.id == null)
                    {
                        return 0;
                    }
                    return (long)first.id;
                }
            }
            throw new ArgumentException("sql语句为空");
        }
        /// <summary>
        /// 执行sql语句
        /// </summary>
        /// <param name="sql">sql语句或者存储过程名称 存储过程需要指定CommandType=CommandType.StoredProcedure</param>
        /// <param name="parameters">动态参数</param>
        /// <param name="transaction">数据库事务对象</param>
        /// <param name="commandTimeout">命令执行超时时间</param> 
        /// <param name="commandType">执行命令类型 text和存储过程</param>
        /// <returns></returns>
        public async Task<int> Execute(string sql, object parameters = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType commandType = CommandType.Text)
        {
            if (!string.IsNullOrEmpty(sql))
            {
                using (var dbConnection = ConnectionFactory.OpenConnection())
                {
                    return await dbConnection.ExecuteAsync(sql, parameters, transaction, commandTimeout, commandType);
                }
            }
            throw new ArgumentException("sql语句为空");
        }
        /// <summary>
        /// 执行sql语句
        /// </summary>
        /// <param name="dbConnection">数据库连接对象</param>
        /// <param name="sql">sql语句或者存储过程名称 存储过程需要指定CommandType=CommandType.StoredProcedure</param>
        /// <param name="parameters">动态参数</param>
        /// <param name="transaction">数据库事务对象</param>
        /// <param name="commandTimeout">命令执行超时时间</param>
        /// <param name="commandType">执行命令类型 text和存储过程</param>
        /// <returns></returns>
        public async Task<int> Execute(IDbConnection dbConnection, string sql, object parameters = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType commandType = CommandType.Text)
        {
            if (!string.IsNullOrEmpty(sql))
            {
                return await dbConnection.ExecuteAsync(sql, parameters, transaction, commandTimeout, commandType);
            }
            throw new ArgumentException("sql语句为空");
        }
        /// <summary>
        /// 执行存储过程
        /// </summary>
        /// <param name="procName">存储过程名称</param>
        /// <param name="parameters">动态参数</param>
        /// <param name="commandTimeout">命令执行超时时间</param>
        /// <returns></returns>
        public async Task<int> ExecuteProc(string procName, object parameters = null, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            if (!string.IsNullOrEmpty(procName))
            {
                using (var dbConnection = ConnectionFactory.OpenConnection())
                {
                    return await dbConnection.ExecuteAsync(procName, parameters, transaction, commandTimeout, CommandType.StoredProcedure);
                }
            }
            throw new ArgumentException("存储过程为空");
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dbConnection">数据库连接对象</param>
        /// <param name="procName">存储过程名称</param>
        /// <param name="parameters">动态参数</param>
        /// <param name="transaction">数据库事务对象</param>
        /// <param name="outTime">命令执行超时时间</param>
        /// <returns></returns>
        public async Task<int> ExecuteProc(IDbConnection dbConnection, string procName, object parameters = null, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            if (!string.IsNullOrEmpty(procName))
            {
                return await dbConnection.ExecuteAsync(procName, parameters, transaction, commandTimeout, CommandType.StoredProcedure);
            }
            throw new ArgumentException("存储过程为空");
        }
        #endregion
    }
}
