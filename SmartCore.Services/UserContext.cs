using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace SmartCore.Services
{
    public class UserContext
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public UserContext(
            IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<int?> GetCurrUserIdAsync()
        {
            var auth = await _httpContextAccessor.HttpContext.AuthenticateAsync();//获取登录用户的AuthenticateResult
            if (auth.Succeeded)
            {
                var userCli = auth.Principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier); //在声明集合中获取ClaimTypes.NameIdentifier 的值就是用户ID
                if (userCli == null || string.IsNullOrEmpty(userCli.Value))
                {
                    return null;
                }
                return Convert.ToInt32(userCli.Value);
            }
            return null;
        }
    }
}
