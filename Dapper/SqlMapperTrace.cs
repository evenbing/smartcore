using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dapper
{
    /// <summary>
    /// Sql执行拦截事件
    /// </summary>
    /// <param name="traceInfo"></param>
    public delegate void BeforeCommandExecute(TraceInfo traceInfo);
    /// <summary>
    /// SQL运行成功后拦截事件
    /// </summary>
    /// <param name="traceInfo"></param>
    public delegate void AfterCommandExecute(TraceInfo traceInfo);
    /// <summary>
    /// DapperSQL执行跟踪
    /// </summary>
    public class SqlMapperTrace
    {
        static BeforeCommandExecute BeforeSqlCommand = null;
        static AfterCommandExecute AfterSqlCommand = null;
        /// <summary>
        /// 设置DapperSql执行拦截事件
        /// </summary>
        /// <param name="beforeExecuteTrace">执行前事件</param>
        /// <param name="afterExecuteTrace">执行后事件</param>
        public static void SetMapperTrace(BeforeCommandExecute beforeExecuteTrace, AfterCommandExecute afterExecuteTrace)
        {
            if (null != BeforeSqlCommand)
                return;
            BeforeSqlCommand = beforeExecuteTrace;
            AfterSqlCommand = afterExecuteTrace;

        }

        /// <summary>
        /// 执行SQL运行前拦截事件
        /// </summary>
        /// <param name="traceInfo"></param>
        internal static void ShellBeforeCommandExecute(TraceInfo traceInfo)
        {
            if (null != BeforeSqlCommand)
                BeforeSqlCommand(traceInfo);
        }

        /// <summary>
        /// 执行SQL运行成功后拦截事件，若SQL执行失败，则此方法不会触发
        /// </summary>
        /// <param name="traceInfo"></param>
        public static void ShellAfterCommandExecute(TraceInfo traceInfo)
        {
            if (null != AfterSqlCommand)
                AfterSqlCommand(traceInfo);
        }
    }

    /// <summary>
    /// Sql执行状态
    /// </summary>
    public enum SqlState
    {
        /// <summary>
        /// 执行中
        /// </summary>
        Start,
        /// <summary>
        /// 完成
        /// </summary>
        End,
        /// <summary>
        /// 失败
        /// </summary>
        Error
    }

    /// <summary>
    /// 跟踪信息
    /// </summary>
    public class TraceInfo
    {
        /// <summary>
        /// 操作识别Key，用用跟踪同一SQL是否执行成功
        /// </summary>
        public string Token { get; set; }
        /// <summary>
        /// SQL语句
        /// </summary>
        public String CommandText { get; set; }
        /// <summary>
        /// sql参数
        /// </summary>
        public object SqlParams { get; set; }

        /// <summary>
        /// 是否正在启动
        /// </summary>
        public SqlState IsStart { get; set; }
        /// <summary>
        /// 执行时间，性能统计，自己可根据两次时间差去实现
        /// </summary>
        public DateTime ExecuteTime { get; set; }

        /// <summary>
        /// 是否执行数据库成功,主要针对增删改操作
        /// </summary>
        public bool IsDbSucc { get; set; }

        /// <summary>
        /// 提示信息
        /// </summary>
        public string Message { get; set; }
    }

}