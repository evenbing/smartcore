﻿
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartCore.Models.DTO;
using SmartCore.Services;
using SmartCore.WebApi;
using System.Threading.Tasks;

namespace SmartCore.WebApi.Controllers
{
    /// <summary>
    /// 令牌
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
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [Route("GetRandomToken")]
        [HttpGet]
        public async Task<IActionResult> GetRandomToken()
        { 
            UserTokenDTO userTokenDTO = new UserTokenDTO();
            userTokenDTO.Email = "wenbin.ye@winbean.com";
            userTokenDTO.Id = 1888999999;
            var token =await _jwtServices.GenerateSecurityToken(userTokenDTO);
            return Ok(token);
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
