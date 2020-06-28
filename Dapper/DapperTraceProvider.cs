using System;
using System.Collections.Generic;
using System.Text;

namespace Dapper
{
    /// <summary>
    /// 数据库跟踪服务
    /// </summary>
  public  class DapperDataTraceProvider
    {
        //[ThreadStatic]
        //internal static IEventManager CurrentEvent = null;
        internal static void InitTrace()
        {
            SqlMapperTrace.SetMapperTrace(BeforeCommandExecute, AfterCommandExecute);
                
        }
        /// <summary>
        /// 执行前事件跟踪信息
        /// </summary>
        /// <param name="traceInfo"></param>
        private static void BeforeCommandExecute(TraceInfo traceInfo)
        { 
        
        }
        /// <summary>
        /// 执行后事件跟踪信息
        /// </summary>
        /// <param name="traceInfo"></param>
        private static void AfterCommandExecute(TraceInfo traceInfo)
        {

        }

        public static void ErrorCommandExecute(TraceInfo traceInfo)
        {

        }
        private static string FormatSql(string sql)
        {
            return "";
        }
    }
}
