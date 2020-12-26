using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using SmartCore.Services;
using System.Threading.Tasks;
using System.Text;
using System;
using SmartCore.Models.DTO;
using SmartCore.Services.User;
/// <summary>
/// 
/// </summary>
namespace SmartCore.WebApi
{
    /// <summary>
    /// 用户信息
    /// </summary>
    //[Authorize]
    [ApiController]
    [Route("[controller]")]
    public class UsersController : BaseApiController
    {
        private static readonly string[] Summaries = new[]
         {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };
        #region 构造函数
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
        #endregion
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpPost, Route("Sign")]
        public async Task<IActionResult> Sign(UserLoginDTO userLoginDTO)
        {
            var result = await _userService.SignIn(userLoginDTO);
            return Ok(result);
        }
        #region 修改密码
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpPost, Route("ChangPassword")]
        public async Task<IActionResult> ChangPassword(UserLoginDTO userLoginDTO)
        {
            var result = await _userService.SignIn(userLoginDTO);
            return Ok(result);
        }
        #endregion
        #region 测试
        // private readonly IHttpClientFactory _clientFactory;IHttpClientFactory _clientFactory
        ///// <summary>
        ///// 
        ///// </summary>
        ///// <returns></returns>
        //[HttpGet]
        //[Route("GetUsersList")]
        //public async Task<IActionResult> GetUsersList()
        //{
        //  var await _userService.SignIn(new UserLoginDTO());
        //   // var id = _userService;// WebHelper.HttpContext?.TraceIdentifier;
        //   //// var auth = await _httpContextAccessor.HttpContext.AuthenticateAsync();//获取登录用户的AuthenticateResult
        //   // if (auth.Succeeded)
        //   // {
        //   //     bool IsAuthenticated = HttpContext.User.Identity.IsAuthenticated;

        //    //        var userCli = auth.Principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
        //    //     return Ok(userCli.Value);
        //    // }
        //    return Ok(null);
        //}
        //[HttpPost]
        //[Route("SaveUserInfo")]
        //public async Task<IActionResult> SaveUserInfo([FromBody]User user)
        //{ 
    //    System.DateTime nowTime = System.DateTime.UtcNow;
    //    long ticks = nowTime.Ticks;
    //    string password = "smartcore";// + new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds(); 
    //    var model = await Task.Run(() => Infrastructure.Security.SHAUtil.Sha512Compute(password));
    //    string securityKey = Guid.NewGuid().ToString("N") + ticks;
    //    string str = Convert.ToBase64String(model.HashData);
    //    byte[] decBytesHashData = Convert.FromBase64String(str);
    //    string strSalt = Convert.ToBase64String(model.Salt);
    //    byte[] decBytesSalt = Convert.FromBase64String(strSalt);
    //    bool result = Infrastructure.Security.SHAUtil.VerifyHash(password, decBytesHashData, decBytesSalt);
    //        //hexString = strB.ToString();
    //        // string password =System.Text.Encoding.UTF8.GetString(model.HashData);
    //        // string salt = System.Text.Encoding.UTF8.GetString(model.Salt);
    //        return Ok(new { hashpassword = str, salt= strSalt, password= password, securitykey= securityKey,result= result
    //});
        //}
        #endregion
    }
}
