
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
        public IActionResult GetControllers()
        {
            //var id=WebHelper.HttpContext?.TraceIdentifier;
           //string ip= WebHelper.Ip;
           // IMediator mediator
            ConcurrentBag<Dictionary<string,List<string>>> list = new ConcurrentBag<Dictionary<string, List<string>>>();
            IEnumerable<System.Type> assembly = null;
            //await Task.Run(() =>
            //{
                 assembly = typeof(Startup).Assembly.GetTypes().AsEnumerable()
     .Where(type => typeof(ControllerBase).IsAssignableFrom(type));
                foreach (var item in assembly)
                {
                    Dictionary<string, List<string>> keyValuePair = new Dictionary<string, List<string>>();
                    keyValuePair.Add(item.Name,item.GetMethods().Where(m => m.IsPublic&&(m.ReturnType.Name.Equals("ActionResult")|| m.ReturnType.Name.Equals("IActionResult") || m.ReturnType.Name.StartsWith("Task"))).Select(s=>s.Name).ToList());//&& !m.IsDefined(typeof(NonActionAttribute))
                list.Add(keyValuePair);
                }
            //});
            var result= new { list = list, ip = "" };
            return Ok(result);
        }
    }
}
