using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SmartCore.Infrastructure.Redis
{
    /// <summary>
    /// 
    /// </summary>
    public class RedisCacheManager : IRedisCacheManager, IDisposable
    {
        #region 全局变量
        /// <summary>
        /// 配置redis host
        /// </summary>
        public static readonly RedisConfigs configInfo = new RedisConfigs();
        /// <summary>
        /// 私有变量 ConnectionMultiplexer
        /// </summary>
        private volatile ConnectionMultiplexer _instance = null;

        /// <summary>
        /// 默认数据库下标
        /// </summary>
        private int defaultDb = configInfo.IsCluster ? 0 : configInfo.DefaultDb;
        /// <summary>
        /// 线程锁
        /// </summary>
        private readonly object _locker = new object();
        #endregion
        #region ConnectionMultiplexer管理
        /// <summary>
        /// 
        /// </summary>
        public ConnectionMultiplexer Instance
        {
            get
            {
                if (_instance == null || !_instance.IsConnected)
                {
                    lock (_locker)
                    {
                        if (_instance == null || !_instance.IsConnected)
                        {

                            _instance = CreateManager();
                        }
                    }
                }
                return _instance;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private ConnectionMultiplexer CreateManager()
        {
            var options = ConfigurationOptions.Parse(string.Join(",", configInfo.Hosts));
            if (!string.IsNullOrEmpty(configInfo.Password))
            {
                options.Password = configInfo.Password;
            }
            options.SyncTimeout = configInfo.SyncTimeout;
            options.ConnectTimeout = configInfo.ConnectTimeout;//15000标识15s
            options.KeepAlive = 180;
            options.AbortOnConnectFail = false;

            _instance = ConnectionMultiplexer.Connect(options);

            //注册如下事件
            //_instance.ConnectionFailed += MuxerConnectionFailed;
            //_instance.ConnectionRestored += MuxerConnectionRestored;
            //_instance.ErrorMessage += MuxerErrorMessage;
            //_instance.ConfigurationChanged += MuxerConfigurationChanged;
            //_instance.HashSlotMoved += MuxerHashSlotMoved;
            //_instance.InternalError += MuxerInternalError;
            return _instance;
        }
        #endregion

        #region 获取redis服务器
        /// <summary>
        /// 获取redis服务器
        /// </summary>
        /// <returns></returns>
        protected IServer GetServer()
        {
            EndPoint[] endpoints = Instance.GetEndPoints();
            IServer result = endpoints.Select(endpoint => Instance.GetServer(endpoint)).FirstOrDefault(server => !server.IsSlave && server.IsConnected);
            if (result == null) throw new InvalidOperationException("Requires exactly one master endpoint (found none)");
            return result;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected List<IServer> GetMasterServers()
        {
            var endpoints = Instance.GetEndPoints();
            var masters =
                endpoints.Select(endpoint => Instance.GetServer(endpoint))
                    .Where(server => !server.IsSlave && server.IsConnected)
                    .ToList();
            return masters;
        }
        #endregion

        #region 转换方法
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        private T ConvertObj<T>(RedisValue value)
        {
            if (!value.HasValue)
            {
                return default(T);
            }
            return JsonConvert.DeserializeObject<T>(value);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="values"></param>
        /// <returns></returns>
        private List<T> ConvertList<T>(RedisValue[] values)
        {
            List<T> result = new List<T>();
            foreach (var item in values)
            {
                var model = ConvertObj<T>(item);
                result.Add(model);
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="values"></param>
        /// <returns></returns>
        private List<string> ConvertList<T>(List<T> values)
        {
            var result = new List<string>();
            foreach (var item in values)
            {
                result.Add(ConvertJson(item));
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="values"></param>
        /// <returns></returns>
        private HashSet<T> ConvertHashSet<T>(RedisValue[] values)
        {
            HashSet<T> result = new HashSet<T>();
            foreach (var item in values)
            {
                var model = ConvertObj<T>(item);
                result.Add(model);
            }
            return result;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        private HashSet<string> ConvertHashSet(RedisValue[] values)
        {
            HashSet<string> result = new HashSet<string>();
            foreach (var item in values)
            {
                result.Add(item);
            }
            return result;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="redisKeys"></param>
        /// <returns></returns>
        private RedisKey[] ConvertRedisKeys(List<string> redisKeys)
        {
            return redisKeys.Select(k => (RedisKey)k).ToArray();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="redisKeys"></param>
        /// <returns></returns>
        private RedisKey[] ConvertRedisKeys(string[] redisKeys)
        {
            return redisKeys.Select(k => (RedisKey)k).ToArray();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        private string ConvertJson<T>(T value)
        {
            if (value != null)
            {
                return JsonConvert.SerializeObject(value);
            }
            return "";
        }
        #endregion

        #region 获取redis服务器当前时间
        /// <summary>
        /// 获取redis服务器当前时间
        /// </summary>
        /// <returns></returns>
        public DateTime GetRedisTime()
        {
            var server = GetServer();
            return server.Time();
        }
        #endregion

        #region 分布式锁 
        /// <summary>
        /// 分布式锁
        /// </summary>
        /// <param name="key"></param>
        /// <param name="timeOut"></param>
        /// <param name="lockValue"></param>
        /// <returns></returns>
        public async Task<string> LockTake(string key, TimeSpan? timeOut)
        {
            var random = new Random();
            string lockValue = string.Format("lock_{0}_{1}", GetRedisTime().Ticks, random.Next());
            if (timeOut == null || timeOut.Value.TotalSeconds < 10)
                timeOut = new TimeSpan(0, 0, 10);
            RedisValue redisValue = ConvertJson(lockValue);
            bool result = await Do(db => db.LockTakeAsync(key, redisValue, timeOut.GetValueOrDefault()));
            return lockValue;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="lockValue"></param>
        /// <returns></returns>
        public async Task<bool> LockRelease(string key, string lockValue)
        {
            var result = await Do(db => db.LockReleaseAsync(key, lockValue));
            return result;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        public async Task<bool> RemoveLock(string key)
        {
            return await Remove(key);
        }
        #endregion

        #region 打开操作库
        /// <summary>
        /// 打开操作库
        /// </summary>
        /// <returns></returns>
        private T Do<T>(Func<IDatabase, T> func)
        {
            int i = 0;
            int retryCount = 3; // 重试次数
            int waitMillisec = 30;
            do
            {
                try
                {
                    //默认操作db0
                    var database = Instance.GetDatabase(defaultDb);
                    return func(database);
                }
                catch (Exception ex)
                {
                    if (i < retryCount + 1)
                    {
                        System.Threading.Thread.Sleep(waitMillisec);
                        i++;
                    }
                    //LogManager.WriteLog("Error", "RedisClusterHelper=>Do=>\n；重试第" + i + "次；\n" + ex.ToString());
                }
            }
            while (i < retryCount + 1);

            return default(T);
        }
        #endregion

        #region 创建事务对象
        /// <summary>
        /// 创建事务对象
        /// </summary>
        /// <returns></returns>
        public ITransaction CreateTransaction()
        {
            return GetDatabase().CreateTransaction();
        }
        #endregion

        #region 获取redis数据库
        /// <summary>
        /// 获取redis数据库
        /// </summary>
        /// <returns></returns>
        public IDatabase GetDatabase()
        {
            return Instance.GetDatabase(defaultDb);
        }
        #endregion

        #region key管理相关
        /// <summary>
        /// 获取key的过期时间
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<TimeSpan?> GetTimeToLive(string key)
        {
            return await Do(db => db.KeyTimeToLiveAsync(key));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<bool> Remove(string key)
        {
            return await Do(db => db.KeyDeleteAsync(key));
        }
        /// <summary>
        /// 判断key是否存在
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<bool> Exists(string key)
        {
            return await Do(db => db.KeyExistsAsync(key));
        }

        /// <summary>
        /// 重新命名key
        /// </summary>
        /// <param name="key">就的redis key</param>
        /// <param name="newKey">新的redis key</param>
        /// <returns></returns>
        public async Task<bool> KeyRename(string key, string newKey)
        {
            return await Do(db => db.KeyRenameAsync(key, newKey));
        }
        #endregion

        #region 执行lua脚本
        /// <summary>
        /// 执行lua脚本
        /// </summary>
        /// <param name="luaBody">lua脚本</param>
        public async Task<RedisResult> ExecLua(string luaBody)
        {
            return await Do(db => db.ScriptEvaluateAsync(luaBody));
        }
        #endregion

        #region 发布&订阅
        /// <summary>
        /// 发布
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="channel"></param>
        /// <param name="msg"></param>
        /// <returns></returns>

        public async Task<long> Publish<T>(string channel, T msg)
        {
            var sub = Instance.GetSubscriber();
            return await sub.PublishAsync(channel, ConvertJson(msg));
        }
        /// <summary>
        /// 订阅
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="onMessageHandler"></param>
        public async Task Subscribe(string channel, Action<string, string> onMessageHandler = null)
        {
            var sub = Instance.GetSubscriber();
            await sub.SubscribeAsync(channel, (chanel, message) =>
            {
                if (onMessageHandler != null)
                {
                    onMessageHandler(channel, message);
                }
            });
        }
        /// <summary>
        /// 取消订阅
        /// </summary>
        /// <param name="channel"></param>
        public async Task UnSubscribe(string channel)
        {
            var sub = Instance.GetSubscriber();
            await sub.UnsubscribeAsync(channel);
        }
        #endregion

        #region 缓存基本操作
        /// <summary>
        /// 获取redis缓存中的值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<T> Get<T>(string key)
        {
            var result = await Do(db => db.StringGetAsync(key));
            return ConvertObj<T>(result);
        }
        /// <summary>
        /// 获取redis缓存中的值 返回类型 字符串
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<string> Get(string key)
        {
            var str = await Get<string>(key);
            if (!string.IsNullOrEmpty(str) && str.StartsWith("\"") && str.EndsWith("\""))
            {
                str = System.Text.RegularExpressions.Regex.Unescape(str);
            }
            return str;
        }
        /// <summary>
        /// 获取redis缓存中多个key下的值
        /// </summary>
        /// <param name="keys"></param>
        /// <returns></returns>
        public async Task<IDictionary<string, string>> GetAll(IEnumerable<string> keys)
        {
            var result = new Dictionary<string, string>();
            foreach (var key in keys)
            {
                var item = await Get(key);
                if (item != null)
                {
                    result.Add(key, item);
                }
            }

            return result;
        }
        /// <summary>
        /// 设置redis值 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expiresAt"></param>
        /// <returns></returns>
        public async Task<bool> Set(string key, string value, DateTime? expiresAt = null)
        {
            return await Set<string>(key, value, expiresAt);
        }
        /// <summary>
        /// 设置redis值 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expiresAt"></param>
        /// <returns></returns>
        public async Task<bool> Set<T>(string key, T value, DateTime? expiresAt = null)
        {
            string json = ConvertJson(value);
            if (expiresAt.HasValue)
            {
                return await Do(db => db.StringSetAsync(key, json, expiresAt - DateTime.Now));
            }
            else
            {
                return await Do(db => db.StringSetAsync(key, json));
            }
        }

        /// <summary>
        /// 设置redis值  如果存在，那么则附加到原来的列表中
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param> 
        /// <returns></returns>
        public async Task<long> SetAppendTo<T>(string key, T value)
        {
            string json = ConvertJson(value);
            return await Do(db => db.StringAppendAsync(key, json));
        }
        #endregion

        #region 自增
        /// <summary>
        /// 数字自增
        /// </summary>
        /// <param name="key"></param>
        /// <param name="expiredTime">过期时间</param>
        /// <param name="val"></param>
        public async Task<long> SetIncr(string key, DateTime? expiredTime, int val = 1)
        {
            var db = Instance.GetDatabase(defaultDb);
            var seq = await db.StringIncrementAsync(key, val);
            if (expiredTime.HasValue)
            {
                await db.KeyExpireAsync(key, expiredTime);
            }
            return seq;
        }
        #endregion

        #region 自减
        /// <summary>
        /// 数字自减
        /// </summary>
        /// <param name="key"></param>
        /// <param name="expiredTime"></param>
        /// <param name="val"></param>
        public async Task<long> SetDecr(string key, DateTime? expiredTime, int val = 1)
        {
            var db = Instance.GetDatabase(defaultDb);
            var seq = await db.StringDecrementAsync(key, val);
            if (expiredTime.HasValue)
            {
                await db.KeyExpireAsync(key, expiredTime);
            }
            return seq;
        }
        #endregion

        #region hash 缓存
        /// <summary>
        /// 
        /// </summary>
        /// <param name="hashId"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expiry"></param>
        public async Task HSet(string hashId, string key, string value, TimeSpan? expiry = null)
        {
            var db = Instance.GetDatabase(defaultDb);
            await db.HashSetAsync(hashId, key, value);
            if (expiry.HasValue)
            {
                await db.KeyExpireAsync(hashId, expiry);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="hashId"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expiry"></param>
        public async Task<bool> HSet<T>(string hashId, string key, T value, TimeSpan? expiry = null)
        {
            var db = Instance.GetDatabase(defaultDb);
            var result = await db.HashSetAsync(hashId, key, ConvertJson(value));
            if (expiry.HasValue)
            {
                await db.KeyExpireAsync(hashId, expiry.Value);
            }
            return result;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="hashId"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<string> HGet(string hashId, string key)
        {
            return await Do(db => db.HashGetAsync(hashId, key));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="hashId"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<T> HGet<T>(string hashId, string key)
        {
            var result = await Do(db => db.HashGetAsync(hashId, key));
            return ConvertObj<T>(result);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hashId"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public async Task<long> HIncr(string hashId, string key, int value = 1)
        {
            return await Do(db => db.HashIncrementAsync(hashId, key, value));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="hashId"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public async Task<long> HDecr(string hashId, string key, int value = 1)
        {
            return await Do(db => db.HashDecrementAsync(hashId, key, value));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="hashId"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<bool> HExists(string hashId, string key)
        {
            return await Do(db => db.HashExistsAsync(hashId, key));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="hashId"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<bool> HRemove(string hashId, string key)
        {
            return await Do(db => db.HashDeleteAsync(hashId, key));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="hashId"></param>
        /// <returns></returns>
        public async Task<bool> HRemoveAll(string hashId)
        {
            return await Do(db => db.KeyDeleteAsync(hashId));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="hashId"></param>
        /// <returns></returns>
        public async Task<List<string>> HKeys(string hashId)
        {
            var result = await Do(db => db.HashKeysAsync(hashId));
            return ConvertList<string>(result);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="hashId"></param>
        /// <returns></returns>
        public async Task<List<T>> HValues<T>(string hashId)
        {
            var result = await Do(db => db.HashValuesAsync(hashId));
            return ConvertList<T>(result);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="hashId"></param>
        /// <returns></returns>
        public async Task<HashSet<T>> HGetAll<T>(string hashId)
        {
            var result = await Do(db => db.HashGetAllAsync(hashId));
            if (result != null)
            {
                return ConvertHashSet<T>(result.Select(h => h.Value).ToArray());
            }
            else
            {
                return null;
            }
        }
        #endregion

        #region set 缓存
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expireIn"></param>
        public async Task<bool> SAdd(string key, string value, TimeSpan? expireIn = null)
        {
            var result = await Do(db => db.SetAddAsync(key, value));
            if (result && expireIn.HasValue)
            {
                await Do(db => db.KeyExpireAsync(key, expireIn));
            }
            return result;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expireIn"></param>
        public async Task<bool> SAdd<T>(string key, T value, TimeSpan? expireIn = null)
        {
            var result = await Do(db => db.SetAddAsync(key, ConvertJson(value)));
            if (result && expireIn.HasValue)
            {
                await Do(db => db.KeyExpireAsync(key, expireIn));
            }
            return result;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="values"></param>
        /// <param name="expireIn"></param>
        public async Task SAddRange<T>(string key, List<T> values, TimeSpan? expireIn = null)
        {
            if (values == null || values.Count == 0) return;

            var storedList = new List<RedisValue>();
            values.ForEach(t => storedList.Add(ConvertJson(t)));
            var result = await Do(db => db.SetAddAsync(key, storedList.ToArray()));
            if (result > 0 && expireIn.HasValue)
            {
                Do(db => db.KeyExpire(key, expireIn));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public async Task<bool> SRem(string key, string value)
        {
            return await Do(db => db.SetRemoveAsync(key, value));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public async Task<bool> SRem<T>(string key, T value)
        {
            return await Do(db => db.SetRemoveAsync(key, ConvertJson(value)));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<HashSet<string>> SGetAll(string key)
        {
            var result = await Do(db => db.SetMembersAsync(key));
            return ConvertHashSet(result);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<HashSet<T>> SGetAll<T>(string key)
        {
            var result = await Do(db => db.SetMembersAsync(key));
            return ConvertHashSet<T>(result);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<long> SCount(string key)
        {
            return await Do(db => db.SetLengthAsync(key));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public async Task<bool> SContains(string key, string value)
        {
            return await Do(db => db.SetContainsAsync(key, value));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public async Task<bool> SContains<T>(string key, T value)
        {
            return await Do(db => db.SetContainsAsync(key, ConvertJson(value)));
        }
        #endregion

        #region List缓存
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="listId"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public async Task<bool> AddItemToList<T>(string listId, T value)
        {
            var result = await Do(db => db.ListRightPushAsync(listId, ConvertJson(value)));
            return result > 0;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="listId"></param>
        /// <param name="list"></param>

        public async Task<bool> AddRangeToList(string listId, List<string> list)
        {
            int successNum = 0;
            foreach (var item in list)
            {
                var result = await Do(db => db.ListRightPushAsync(listId, item));
                if (result > 0)
                {
                    successNum++;
                }
            }
            return successNum>0;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="listId"></param>
        /// <param name="list"></param>
        public async Task<bool> AddRangeToList<T>(string listId, List<T> list)
        {
            int successNum = 0;
            foreach (var item in list)
            {
                var result =await Do(db => db.ListRightPushAsync(listId, ConvertJson(item))); 
                if (result > 0)
                {
                    successNum++;
                }
            }
            return successNum > 0;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="listId"></param>
        /// <returns></returns>
        public async Task<long> GetListCount(string listId)
        {
            return await Do(db => db.ListLengthAsync(listId));
        } 

        #region 队列操作
        /// <summary>
        /// 入列
        /// </summary>
        /// <param name="listId"></param>
        /// <param name="value"></param>
        public async Task<long> EnqueueItemOnList(string listId, string value)
        {
            var values=await Do(db =>db.ListLeftPushAsync(listId, value));
            return values;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="listId"></param>
        /// <returns></returns>
        public async Task<string> DequeueItemFromList(string listId)
        {
            return await Do(db => db.ListLeftPopAsync(listId));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="listId"></param>
        /// <returns></returns>
        public async Task<T> DequeueItemFromList<T>(string listId)
        {
            var item =await Do(db => db.ListLeftPopAsync(listId));
            return ConvertObj<T>(item);
        }
        #endregion
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="listId"></param>
        /// <returns></returns>
        public async Task<List<T>> ListGetAll<T>(string listId)
        {
            var values =await Do(db => db.ListRangeAsync(listId));
            return ConvertList<T>(values);
        }
        #endregion

        #region SortedSet缓存
        /// <summary>
        /// SortedSet缓存
        /// </summary>
        /// <param name="setId"></param>
        /// <returns></returns>
        public async Task<long> SortedSetCount(string setId)
        {
            return await Do(db => db.SortedSetLengthAsync(setId));//SortedSetLengthAsync
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="setId"></param>
        /// <param name="value"></param>
        /// <param name="score"></param>
        /// <returns></returns>
        public async Task<bool> SortedSetAdd<T>(string setId, T value, double? score = null)
        {
            var storedValue = ConvertJson(value);
            var realScore = score ?? GetLexicalScore(storedValue);
            return await Do(db => db.SortedSetAddAsync(setId, storedValue, realScore));//SortedSetAdd
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private static double GetLexicalScore(string value)
        {
            if (String.IsNullOrEmpty(value))
                return 0;

            var lexicalValue = 0;
            if (value.Length >= 1)
                lexicalValue += value[0] * (int)Math.Pow(256, 3);

            if (value.Length >= 2)
                lexicalValue += value[1] * (int)Math.Pow(256, 2);

            if (value.Length >= 3)
                lexicalValue += value[2] * (int)Math.Pow(256, 1);

            if (value.Length >= 4)
                lexicalValue += value[3];

            return lexicalValue;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="setId"></param>
        /// <param name="valueList"></param>
        /// <param name="score"></param>
        /// <returns></returns>
        public async Task<bool> SortedSetAddRange<T>(string setId, List<T> valueList, double? score = null)
        {
            if (valueList == null || valueList.Count == 0) return false;

            var storedValues = ConvertList(valueList);
            var realScore = score ?? GetLexicalScore(storedValues[0]);
            var array = storedValues.Select(i => new SortedSetEntry(i, realScore)).ToArray();
            return await Do(db => db.SortedSetAddAsync(setId, array)) > 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="setId"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public async Task<bool> SortedSetRemove<T>(string setId, T value)
        {
            return await Do(db => db.SortedSetRemoveAsync(setId, ConvertJson(value)));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="setId"></param>
        /// <returns></returns>
        public async Task<List<T>> SortedSetMembers<T>(string setId)
        {
            var values = await Do(db => db.SortedSetRangeByRankAsync(setId));
            return ConvertList<T>(values);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="setId"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public async Task<bool> SortedSetContainsItem<T>(string setId, T value)
        {
            var storedValue = ConvertJson(value);
            var values = await Do(db => db.SortedSetRangeByRankAsync(setId));
            return values != null ? values.Select(i => i).ToList().Contains(storedValue) : false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="setId"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public async Task<long> SortedSetItemIndex<T>(string setId, T value)
        {
            var storedValue = ConvertJson(value);
            var values = await Do(db => db.SortedSetRangeByRankAsync(setId));
            return values != null ? values.Select(i => i.ToString()).ToList().IndexOf(storedValue) : 0;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="set"></param>
        /// <param name="beginRank"></param>
        /// <param name="endRank"></param>
        /// <returns></returns>
        public async Task<List<string>> GetRangeFromSortedSet(string set, int beginRank, int endRank)
        {
            var values = await Do(db => db.SortedSetRangeByRankAsync(set, beginRank, endRank));
            return values != null ? values.Select(i => i.ToString()).ToList() : new List<string>();
        }
        #endregion

        #region 模糊查找redis key
        ///// <summary>
        ///// 查找匹配的redis cache key
        ///// </summary>
        ///// <param name="pattern"></param>
        ///// <param name="pageSize"></param>
        ///// <returns></returns>
        //public async Task<List<string>> SearchKeys(string pattern, int pageSize = 3000)
        //{
        //    //Task.CompletedTask;
        //    CancellationToken cancellationTokenSource = new CancellationToken();
        //    var result = new List<string>();
        //    //RedisValue redisValue = pattern;
        //    //redis 3.0 cluster目前有个缺陷是M-S中M服务器key过期，S不会自动过去key,所以StackExchange.redis未做读写分离，所有命令全部落在M上执行
        //    foreach (var server in GetMasterServers())
        //    {

        //        var keysList = await server.KeysAsync(defaultDb, pattern: pattern, pageSize: pageSize).GetAsyncEnumerator(cancellationTokenSource);
        //        foreach (var redisKey in keysList)
        //        {
        //            result.Add(redisKey);
        //        }
        //        server.FlushDatabase();
        //    }

        //    return result;
        //}

        //        var pattern = "keyword*";
        //        var redisResult = _db.ScriptEvaluateAsync(LuaScript.Prepare(
        //                        //Redis的keys模糊查询：
        //                        " local res = redis.call(‘KEYS‘, @keypattern) " +
        //                        " return res "), new { @keypattern = pattern });

        //if (!redisResult.IsNull) {
        //　　_db.KeyDelete((string[]) redisResult);  
        //            }
        #endregion

        #region Dispose
        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            if (Instance != null && Instance.IsConnected)
            {
                Instance.Close();
            }

            if (Instance != null)
            {
                Instance.Dispose();
            }
        }
        #endregion
    }
}
