using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using SmartCore.Services;
using System.Threading.Tasks;
/// <summary>
/// 
/// </summary>
namespace SmartCore.WebApi
{
    /// <summary>
    /// 
    /// </summary>
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
        private readonly IUserService _userService;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="userService"></param>
        public UsersController(IUserService userService)
        {
            _userService = userService;
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
          var await _userService.SignIn(new UserLoginDTO());
           // var id = _userService;// WebHelper.HttpContext?.TraceIdentifier;
           //// var auth = await _httpContextAccessor.HttpContext.AuthenticateAsync();//获取登录用户的AuthenticateResult
           // if (auth.Succeeded)
           // {
           //     bool IsAuthenticated = HttpContext.User.Identity.IsAuthenticated;

            //        var userCli = auth.Principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            //     return Ok(userCli.Value);
            // }
            return Ok(null);
        }
        //[HttpPost]
        //[Route("SaveUserInfo")]
        //public async Task<IActionResult> SaveUserInfo([FromBody]User user)
        //{ 
            
        //}
    }
}
