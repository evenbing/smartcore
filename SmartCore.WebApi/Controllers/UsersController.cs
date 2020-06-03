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
        private readonly IHttpContextAccessor _httpContextAccessor;
        public UsersController(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
    // private readonly IHttpClientFactory _clientFactory;IHttpClientFactory _clientFactory
            [HttpGet]
        [Route("GetUsersList")]
        public async Task<IActionResult> GetUsersList()
        {
            var auth = await _httpContextAccessor.HttpContext.AuthenticateAsync();//获取登录用户的AuthenticateResult
            if (auth.Succeeded)
            {
                bool IsAuthenticated = HttpContext.User.Identity.IsAuthenticated;
                
                   var userCli = auth.Principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
                return Ok(userCli.Value);
            }
                return Ok(null);
        }
    
        //private IUserService _userService;

        //public UsersController(IUserService userService)
        //{
        //    _userService = userService;
        //}

        //[AllowAnonymous]
        //[HttpPost("authenticate")]
        //public IActionResult Authenticate([FromBody]AuthenticateModel model)
        //{
        //    var user = _userService.Authenticate(model.Username, model.Password);

        //    if (user == null)
        //        return BadRequest(new { message = "Username or password is incorrect" });

        //    return Ok(user);
        //}

        //[HttpGet]
        //public IActionResult GetAll()
        //{
        //    var users = _userService.GetAll();
        //    return Ok(users);
        //}
    }
}
