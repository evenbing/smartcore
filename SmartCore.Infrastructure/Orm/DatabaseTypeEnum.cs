using System;
using System.Collections.Generic;
using System.Text;

namespace SmartCore.Infrastructure.Orm
{
    public enum  DatabaseTypeEnum
    {
        
            SqlServer,  //SQLServer数据库
            MySql,      //Mysql数据库
            Npgsql,     //PostgreSQL数据库
            Oracle,     //Oracle数据库
            Sqlite,     //SQLite数据库
            DB2         //IBM DB2数据库
        
    }
}
