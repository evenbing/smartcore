using SmartCore.Infrastructure.Orm;
using SmartCore.Models.Entity;
using SmartCore.Repository.Base;
using SmartCore.Repository.Base.Impl;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartCore.Repository.Sys.Impl
{
    /// <summary>
    /// 
    /// </summary>

    [DatatSourceSlave]
    public class SysDictRepository: BaseRepository<SysDictEntity>, ISysDictRepository
    {

    }
}
