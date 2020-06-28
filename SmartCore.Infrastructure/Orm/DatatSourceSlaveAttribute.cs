using System;
using System.Collections.Generic;
using System.Text;

namespace SmartCore.Infrastructure.Orm
{
    [AttributeUsage(AttributeTargets.Class|AttributeTargets.Method)]
   public class DatatSourceSlaveAttribute:Attribute
    {
    }
}
