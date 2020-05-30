using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartCore.Services.Sys
{
    public interface ISysDictServices
    {
        Task<List<SmartCore.Models.Entity.SysDictEntity>> QueryAllList();
    }
}
