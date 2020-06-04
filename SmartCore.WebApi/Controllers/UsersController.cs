using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
//using WebApi.Services;
//using WebApi.Models;
using System.Linq;
using System.Net.Http;
using SmartCore.Services;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using SmartCore.Infrastructure;

namespace SmartCore.WebApi
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class UsersController : BaseApiController
    {
        private static readonly string[] Summaries = new[]
         {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };
        /// <summary>
        /// 
        /// </summary>
        private readonly IHttpContextAccessor _httpContextAccessor;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="httpContextAccessor"></param>
        public UsersController(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
    // private readonly IHttpClientFactory _clientFactory;IHttpClientFactory _clientFactory
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("GetUsersList")]
        public async Task<IActionResult> GetUsersList()
        {
            var id = WebHelper.HttpContext?.TraceIdentifier;
            var auth = await _httpContextAccessor.HttpContext.AuthenticateAsync();//获取登录用户的AuthenticateResult
            if (auth.Succeeded)
            {
                bool IsAuthenticated = HttpContext.User.Identity.IsAuthenticated;
                
                   var userCli = auth.Principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
                return Ok(userCli.Value);
            }
                return Ok(null);
        }
        //[HttpPost]
        //[Route("SaveUserInfo")]
        //public async Task<IActionResult> SaveUserInfo([FromBody]User user)
        //{ 
            
        //}
    }
}
