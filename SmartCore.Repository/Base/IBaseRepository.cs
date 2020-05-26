using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SmartCore.Repository.Base
{
    /// <summary>
    /// 数据库基本的CRUD 接口定义
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public interface IBaseRepository<TEntity> where TEntity : class, new()
    {
        /// <summary>
        /// 
        /// </summary>
        string TableName { get; set; }
        #region 新增
        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<int> Add(TEntity entity);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="listEntity"></param>
        /// <returns></returns>
        Task<int> BatchAdd(List<TEntity> listEntity);
        #endregion

        #region 修改
        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<bool> Update(TEntity entity);
        /// <summary>
        /// 批量更新实体数据
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        Task<bool> Update(List<TEntity> entities);

        /// <summary>
        /// 根据sql语句来更新
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="paramters"></param>
        /// <returns></returns>
        Task<bool> UpdateBySql(string sql, object paramters = null);
        /// <summary>
        /// 字段及对应的值
        /// </summary>
        /// <param name="keyValuePairs"></param>
        /// <returns></returns>
        Task<bool> UpdatePartialFields(Dictionary<string, object> keyValuePairs);
        #endregion

        #region 删除
        /// <summary>
        /// 删除当前实体的表数据
        /// </summary>
        /// <returns></returns>
        bool Delete(TEntity entity);
        /// <summary>
        /// 根据主键id删除的表数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        bool Delete(int id);
        /// <summary>
        /// 删除多个主键id的表数据
        /// </summary>
        /// <param name="idList">主键id列表</param>
        /// <returns></returns>
        bool Delete(List<int> idList);
        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        bool Delete(List<TEntity> entities);
        #endregion
        
        #region 查询

        #region 获取第一行第一列的数据
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TypeValue"></typeparam>
        /// <param name="sql">sql语句或者存储过程名称 存储过程需要指定CommandType=CommandType.StoredProcedure</param>
        /// <param name="parameters">动态参数</param> 
        /// <param name="commandTimeout">命令执行超时时间</param>
        /// <param name="commandType">命令脚本类型 一般分为Command.Text和CommandType.StoredProcedure</param>
        /// <returns></returns>
        Task<T> ExecuteScalar<T>(string sql, dynamic parameters = null,
         int? commandTimeout = null, CommandType? commandType = null);
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TypeValue"></typeparam>
        /// <param name="conn">数据库连接对象</param>
        /// <param name="sql">sql语句或者存储过程名称 存储过程需要指定CommandType=CommandType.StoredProcedure</param>
        /// <param name="parameters">动态参数</param>
        /// <param name="dbTransaction">数据库事务对象</param>
        /// <param name="commandTimeout">命令执行超时时间</param>
        /// <param name="commandType">命令脚本类型 一般分为Command.Text和CommandType.StoredProcedure</param>
        /// <returns></returns>
        Task<T> ExecuteScalar<T>(IDbConnection conn, string sql, dynamic parameters = null, IDbTransaction dbTransaction = null,
     int? commandTimeout = null, CommandType? commandType = null);
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="parameters">动态参数</param>
        /// <param name="command">执行命令类型 text和存储过程</param>
        /// <returns></returns>
        Task<string> ExecuteScalarToStr(string sql, object parameters = null, CommandType command = CommandType.Text);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="conn">数据库连接对象</param>
        /// <param name="sql">sql语句</param>
        /// <param name="parameters">动态参数</param>
        /// <param name="command">执行命令类型 text和存储过程</param>
        /// <returns></returns>
        Task<string> ExecuteScalarToStr(IDbConnection conn, string sql, object parameters = null, CommandType command = CommandType.Text);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="parameters">动态参数</param>
        /// <param name="command">执行命令类型 text和存储过程</param>
        /// <returns></returns>
        Task<int> ExecuteScalarToInt(string sql, object parameters = null, CommandType command = CommandType.Text);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="conn">数据库连接对象</param>
        /// <param name="sql">sql语句</param>
        /// <param name="parameters">动态参数</param>
        /// <param name="command">执行命令类型 text和存储过程</param>
        /// <returns></returns>
        Task<int> ExecuteScalarToInt(IDbConnection conn, string sql, object parameters = null, CommandType command = CommandType.Text);

        #endregion

        #region 获取单条实体信息
        /// <summary>
        /// 根据主键id获取单条实体信息
        /// </summary>
        /// <param name="id">主键id</param>
        /// <returns></returns>
        Task<TEntity> Get(dynamic id);
        /// <summary>
        /// 根据sql获取当前实体类
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="parameters">动态参数</param>
        /// <returns></returns>
        Task<TEntity> Get(string sql, object parameters = null);
        /// <summary>
        /// 根据sql获取当前实体类
        /// </summary>
        /// <param name="dbConnection">连接字符串</param>
        /// <param name="sql">sql语句</param>
        /// <param name="parameters">动态参数</param>
        /// <returns></returns>
        Task<TEntity> Get(IDbConnection dbConnection, string sql, object parameters = null);
        /// <summary>
        /// 根据sql语句获取泛型实体类
        /// </summary>
        /// <typeparam name="TEntity">泛型类</typeparam>
        /// <param name="sql">sql语句</param>
        /// <param name="parameters">动态参数</param>
        /// <returns></returns>
        Task<T> Get<T>(string sql, object parameters = null);
        /// <summary>
        /// 根据sql获取泛型实体类
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        ///  <param name="dbConnection">数据库连接对象</param>
        /// <param name="sql">sql语句</param>
        /// <param name="parameters">动态参数</param>
        /// <returns></returns>
        Task<T> Get<T>(IDbConnection dbConnection, string sql, object parameters = null);
        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task<List<TEntity>> QueryAll();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="whereExpression"></param>
        /// <returns></returns>
        Task<List<TEntity>> Query(Expression<Func<TEntity, bool>> whereExpression);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        Task<List<TEntity>> QuerySql(string sql,object parameters = null);
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        int GetCount();

        #region 获取Datable
        /// <summary>
        /// 获取Datable
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        Task<DataTable> QuaryTable(string sql, object parameters = null);
        /// <summary>
        /// 获取Datable
        /// </summary>
        /// <param name="dbConnection"></param>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        Task<DataTable> QuaryTable(IDbConnection dbConnection, string sql, object parameters = null);

        #endregion

        #region 根据表名获取列名
        /// <summary>
        /// 根据表名获取列名
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        Task<List<string>> GetColumnNameList(string tableName);

        #endregion

        #endregion

        #region 执行sql Excute SQL
        /// <summary>
        /// 执行sql 返回自增id，一般由于sql语句插入到数据表中
        /// </summary>
        /// <param name="sql">执行sql</param>
        /// <param name="parameters">动态参数</param>
        /// <returns>返回自增id</returns>
        Task<long> ExcuteAndReturnId(string sql, object parameters = null);
        /// <summary>
        /// 执行sql语句
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="parameters">动态参数</param>
        /// <param name="outTime">命令执行超时时间</param> 
        /// <returns></returns>
        Task<int> Execute(string sql, object parameters = null, int? outTime = null);
        /// <summary>
        /// 执行sql语句
        /// </summary>
        /// <param name="conn">数据库连接对象</param>
        /// <param name="sql">sql语句</param>
        /// <param name="parameters">动态参数</param>
        /// <param name="outTime">命令执行超时时间</param>
        /// <returns></returns>
        Task<int> Execute(IDbConnection conn, string sql, object parameters = null, int? outTime = null, IDbTransaction transaction = null);
        /// <summary>
        /// 执行存储过程
        /// </summary>
        /// <param name="procName">存储过程名称</param>
        /// <param name="parameters">动态参数</param>
        /// <param name="outTime">命令执行超时时间</param>
        /// <returns></returns>
        Task<int> ExecuteProc(string procName, object parameters = null, int? outTime = null);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="conn">数据库连接对象</param>
        /// <param name="procName">存储过程名称</param>
        /// <param name="parameters">动态参数</param>
        /// <param name="transaction">数据库事务对象</param>
        /// <param name="outTime">命令执行超时时间</param>
        /// <returns></returns>
        Task<int> ExecuteProc(IDbConnection conn, string procName, object parameters = null, IDbTransaction transaction = null, int? outTime = null);
        #endregion
    }
}
