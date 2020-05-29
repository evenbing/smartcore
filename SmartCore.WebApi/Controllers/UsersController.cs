using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
//using WebApi.Services;
//using WebApi.Models;
using System.Linq;
using System.Net.Http;

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
       // private readonly IHttpClientFactory _clientFactory;IHttpClientFactory _clientFactory
        [HttpGet]
        [Route("GetUsersList")]
        public ActionResult GetUsersList()
        {
            return Ok(Summaries);
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
