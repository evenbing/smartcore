using System;
using System.Collections.Generic;
using System.Text;

namespace Dapper.Contrib
{
    public class PageModel
    {
        /// <summary>
        /// 当前页
        /// </summary>
        public int PageIndex { get; set; }
        /// <summary>
        /// 每页大小
        /// </summary>
        public int PageSize { get; set; }
        /// <summary>
        /// 排序字段
        /// </summary>
        public string OrderBy { get; set; }
        /// <summary>
        /// 返回字段逗号分隔
        /// </summary>
        public string ReturnFields { get; set; }
        /// <summary>
        /// sql条件
        /// </summary>
        public string ConditionWhere { get; set; }
        /// <summary>
        /// 动态参数
        /// </summary>
        public object DynamicParam { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Prefix { get; set; }
        /// <summary>
        /// 表名
        /// </summary>
        public string Tables { get; set; }
        /// <summary>
        /// 分组字段
        /// </summary>
        public string GroupBy { get; set; }
    }
}
