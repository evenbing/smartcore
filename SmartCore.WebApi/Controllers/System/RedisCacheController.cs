
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using SmartCore.Infrastructure;
using SmartCore.WebApi;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Route("[controller]")]
    [ApiController]
    public class RedisCacheController : BaseApiController
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Route("GetRedisValue")]
        [HttpGet]
        public async Task<IActionResult> GetRedisValue(string cacheKey)
        {
            List<int> list = new List<int>();
            for (int i = 0; i < 10000; i++)
            {
                list.Add(i);
               await CacheManager.Instance.HRemove(i.ToString(), "cache:test:");
            }
            //var result= CacheManager.Instance.HGet
            return Ok();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Route("EnqueueItemOn")]
        [HttpGet]
        public async Task<IActionResult> EnqueueItemOn(string cacheKey)
        {
            List<int> list = new List<int>();
            for (int i = 0; i < 10000; i++)
            {
                list.Add(i);
                await CacheManager.Instance.EnqueueItemOnList(cacheKey+":"+i.ToString(), "cache:test:"+i.ToString());
            }
            //var result= CacheManager.Instance.HGet
            return Ok();
        }        /// <summary>
                 /// 
                 /// </summary>
                 /// <returns></returns>
        [Route("DequeueItem")]
        [HttpGet]
        public async Task<IActionResult> DequeueItem(string cacheKey)
        {
            List<int> list = new List<int>();
            for (int i = 0; i < 10000; i++)
            {
                list.Add(i);
                await CacheManager.Instance.DequeueItemFromList(cacheKey + ":" + i.ToString());
            }
            //var result= CacheManager.Instance.HGet
            return Ok();
        }
    }
}
