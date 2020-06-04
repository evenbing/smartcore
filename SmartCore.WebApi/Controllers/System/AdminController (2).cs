
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using SmartCore.Infrastructure;
using SmartCore.WebApi;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Route("[controller]")]
    [ApiController]
    public class AdminController : BaseApiController
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Route("GetControllers")]
        [HttpGet]  
        public async Task<IActionResult> GetControllers()
        {
            var id=WebHelper.HttpContext?.TraceIdentifier;
           // IMediator mediator
            ConcurrentBag<string> list = new ConcurrentBag<string>();
            IEnumerable<System.Type> assembly = null;
            await Task.Run(() =>
            {
                 assembly = typeof(Startup).Assembly.GetTypes().AsEnumerable()
     .Where(type => typeof(ControllerBase).IsAssignableFrom(type));
                foreach (var item in assembly)
                {
                    list.Add(item.Name);
                }
            });
            return Ok(list);
        }
    }
}
