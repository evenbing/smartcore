using System; 
using Microsoft.AspNetCore.Mvc;

namespace SmartCore.WebApi.Controllers
{
    /// <summary>
    /// 健康检查
    /// </summary>
    [Route("[controller]")]
    [ApiController]
    public class HeathController : ControllerBase
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet("status")]
        public IActionResult Status() => Ok();
    }
}