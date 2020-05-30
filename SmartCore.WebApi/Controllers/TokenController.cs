
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using SmartCore.Models.DTO;
using SmartCore.Services;

namespace WebApi.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Route("[controller]")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
         {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };
        private IConfiguration _config;

        public TokenController(IConfiguration config)
        {
            _config = config;
        }
        [Route("GetRandomToken")]
        [HttpGet]
        public string GetRandomToken()
        {
            var jwt = new JwtService();
            UserTokenDTO userTokenDTO = new UserTokenDTO();
            var token = jwt.GenerateSecurityToken(userTokenDTO);
            return token;
        }

        [Route("GetRefreshToken")]
        [HttpGet]
        public string GetRefreshToken()
        {
            var jwt = new JwtService(); 
            UserTokenDTO userTokenDTO = new UserTokenDTO();
            userTokenDTO.Email = "wenbin.ye@winbean.com";
            var token = jwt.GenerateSecurityToken(userTokenDTO);
            return token;
        }
    }
}
