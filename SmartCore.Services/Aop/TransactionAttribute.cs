using System;
using System.Collections.Generic;
using System.Text;
using System.Transactions;

namespace SmartCore.Services.Attributes
{
    /// <summary>
    /// 开启事务属性
    /// </summary>
    /// <example>[Transaction]</example>
    /// <remarks>利用Autofac DynamicProxy+Castle 实现AOP事务
    /// 优势主要表现在:
    ///     1.将通用功能从业务逻辑中抽离出来，就可以省略大量重复代码，有利于代码的操作和维护。
    ///     2.在软件设计时，抽出通用功能（切面），有利于软件设计的模块化，降低软件架构的复杂程度。也就是说通用的功能就是一个单独的模块，在项目的主业务里面是看不到这些通用功能的设计代码的。
    /// </remarks>
    [AttributeUsage(AttributeTargets.Method, Inherited = true)]
    public class TransactionAttribute : Attribute
    {
        /// <summary>
        /// 超时时间
        /// </summary>
        public int Timeout { get; set; }

        /// <summary>
        /// 事务范围
        /// </summary>
        public TransactionScopeOption ScopeOption { get; set; }

        /// <summary>
        /// 事务隔离级别
        /// </summary>
        public IsolationLevel IsolationLevel { get; set; }

        public TransactionAttribute()
        {
            Timeout = 60;
            ScopeOption = TransactionScopeOption.Required;
            IsolationLevel = IsolationLevel.ReadCommitted;
        }
    }
}
