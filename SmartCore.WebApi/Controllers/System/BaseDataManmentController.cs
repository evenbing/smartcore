using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartCore.Services.Sys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks; 

namespace SmartCore.WebApi.Controllers.System
{
    /// <summary>
    /// 
    /// </summary>
    //[Authorize]
    [Route("[controller]")]
    [ApiController]
    public class BaseDataManmentController:BaseApiController
    {
        /// <summary>
        /// 
        /// </summary>
        private ISysDictServices _sysDictServices;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sysDictServices"></param>
        public BaseDataManmentController(ISysDictServices sysDictServices)
        {
            _sysDictServices = sysDictServices;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Route("DicList")]
        [HttpGet]
        public async Task<IActionResult> DicList()
        { 
            var list =await _sysDictServices.QueryAllList();
            return Ok(list); 
        }
    }
}
