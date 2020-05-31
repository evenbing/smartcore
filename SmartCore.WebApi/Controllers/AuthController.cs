using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Jwt.Controllers
{
    [ApiController]
    public class AuthController : ControllerBase
    {
        [AllowAnonymous]
        [HttpGet]
        [Route("api/nopermission")]
        public IActionResult NoPermission()
        {
            return Forbid("No Permission!");
        } 
        [HttpGet]
        [Route("api/GetAuth")]
        [Authorize("Permission")]
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
    }
}