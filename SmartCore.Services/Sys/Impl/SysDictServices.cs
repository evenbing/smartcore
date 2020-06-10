
using SmartCore.Repository.Sys;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SmartCore.Services.Sys
{
    public class SysDictServices: ISysDictServices
    {
        private ISysDictRepository _sysDictRepository;
        public SysDictServices(ISysDictRepository sysDictRepository) {
            _sysDictRepository = sysDictRepository;
        }
        /// <summary>
        /// 获取所有字典数据列表
        /// </summary>
        /// <returns></returns>
        public async Task<List<SmartCore.Models.Entity.SysDictEntity>> QueryAllList()
        {
            return await _sysDictRepository.QueryAllList();
        }
    }
}
