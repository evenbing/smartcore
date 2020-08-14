using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SmartCore.WebApi.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Route("[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        #region 用户登录
        #endregion

        #region 用户退出登录
        #endregion

        #region 获取登录验证码
        public async Task<IActionResult> GetVerifyCode() {
            return Ok();
        }
        #endregion

        #region 测试
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        [Route("nopermission")]
        public IActionResult NoPermission()
        {
            return Forbid("No Permission!");
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("GetAuth")]
        //[Authorize("Permission")]
        public async Task<IActionResult> GetAuth()
        {
            //这是获取自定义参数的方法
            var auth = await HttpContext.AuthenticateAsync();
            var claims = auth.Principal.Claims;
            var userName = claims.FirstOrDefault(t => t.Type.Equals(ClaimTypes.NameIdentifier))?.Value;
            var role = claims.FirstOrDefault(t => t.Type.Equals("Role"))?.Value;
            return Ok(new string[] { "这个接口有管理员权限才可以访问", $"userName={userName}", $"Role={role}" });
        }
        /// <summary>
        /// 模拟登陆校验，因为是模拟，所以逻辑很‘模拟’
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="pwd"></param>
        /// <param name="role"></param>
        /// <returns></returns>
        private bool CheckAccount(string userName, string pwd, out string role)
        {
            role = "user";

            if (string.IsNullOrEmpty(userName))
                return false;

            if (userName.Equals("admin"))
                role = "admin";

            return true;
        }
        #endregion
    }
}