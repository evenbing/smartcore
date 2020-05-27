using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
//using WebApi.Services;
//using WebApi.Models;
using System.Linq;
using System.Net.Http;

namespace WebApi.Controllers
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
        private readonly IHttpClientFactory _clientFactory;
        [HttpGet]
        [Route("GetUsersList")]
        public ActionResult GetUsersList(IHttpClientFactory _clientFactory)
        {
            return Ok(Summaries);
        }
        //[HttpGet]
        //public IEnumerable<WeatherForecast> Get()
        //{
        //    var rng = new Random();
        //    return Enumerable.Range(1, 5).Select(index => new WeatherForecast
        //    {
        //        Date = DateTime.Now.AddDays(index),
        //        TemperatureC = rng.Next(-20, 55),
        //        Summary = Summaries[rng.Next(Summaries.Length)]
        //    })
        //    .ToArray();
        //}
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
