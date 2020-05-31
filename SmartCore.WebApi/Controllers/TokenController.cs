
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartCore.Models.DTO;
using SmartCore.Services;
using SmartCore.WebApi;

namespace WebApi.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Route("[controller]")]
    [ApiController]
    public class TokenController : BaseApiController
    {
        //private static readonly string[] Summaries = new[]
        // {
        //    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        //};
        /// <summary>
        /// Jwt 服务
        /// </summary>
        private readonly IJwtServices _jwtServices;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="jwtServices"></param>
        public TokenController(IJwtServices jwtServices)
        {
            //_config = config;
            _jwtServices = jwtServices;
        }
        [Route("GetRandomToken")]
        [HttpGet]
        public string GetRandomToken()
        { 
            UserTokenDTO userTokenDTO = new UserTokenDTO();
            userTokenDTO.Email = "wenbin.ye@winbean.com";
            var token = _jwtServices.GenerateSecurityToken(userTokenDTO);
            return token;
        }
        //[Route("GetTokenForTest")]
        //[HttpGet]
        //public string GetTokenForTest()
        //{
        //    var jwt = new JwtService();
        //    UserTokenDTO userTokenDTO = new UserTokenDTO();
        //    userTokenDTO.Email = "admin@winbean.com";
        //    var token = jwt.GenerateSecurityToken(userTokenDTO);
        //    return token;
        //}
        //[Route("PostRefreshToken")]
        //[HttpPost]
        //public async Task<string> PostRefreshToken(string token)
        //{
        //    var jwt = new JwtService(); 
       
        //    var tokenInfo =await jwt.RefreshToken(token);
        //    return token;
        //}
    }
}
