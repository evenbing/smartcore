using SmartCore.Infrastructure.Redis;
using System; 
/// <summary>
/// 
/// </summary>
namespace SmartCore.Infrastructure
{
    /// <summary>
    /// 缓存管理实现类 单例模式
    /// </summary>
   public class CacheManager
    {/// <summary>
     /// 
     /// </summary>
        public static int _Timeout = 30;
        /// <summary>
        /// 
        /// </summary>
        public static int Timeout { get { return _Timeout; } }
        // 定义一个静态变量来保存类的实例
        private static IRedisCacheManager _RedisCacheManager;
        private static readonly object _locker = new object();

        /// <summary>
        /// 单例模式
        /// </summary>
        public static IRedisCacheManager Instance
        {
            get
            {
                if (_RedisCacheManager == null )
                {
                    lock (_locker)
                    {
                        return _RedisCacheManager ?? (_RedisCacheManager = new RedisCacheManager());
                    }
                }
                return _RedisCacheManager;
            }
        }
    }
}
