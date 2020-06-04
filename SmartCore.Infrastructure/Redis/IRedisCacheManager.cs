using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SmartCore.Infrastructure.Redis
{
    public interface IRedisCacheManager
    {
        #region key管理相关
        /// <summary>
        /// 获取key的过期时间
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        Task<TimeSpan?> GetTimeToLive(string key);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        Task<bool> Remove(string key);
        /// <summary>
        /// 判断key是否存在
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        Task<bool> Exists(string key);

        /// <summary>
        /// 重新命名key
        /// </summary>
        /// <param name="key">就的redis key</param>
        /// <param name="newKey">新的redis key</param>
        /// <returns></returns>
        Task<bool> KeyRename(string key, string newKey);
        #endregion

        #region 分布式锁 
        /// <summary>
        /// 分布式锁
        /// </summary>
        /// <param name="key"></param>
        /// <param name="timeOut"></param>
        /// <param name="lockValue"></param>
        /// <returns></returns>
        Task<string> LockTake(string key, TimeSpan? timeOut);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="lockValue"></param>
        /// <returns></returns>
        Task<bool> LockRelease(string key, string lockValue);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        Task<bool> RemoveLock(string key);
        #endregion

        #region 执行lua脚本
        /// <summary>
        /// 执行lua脚本
        /// </summary>
        /// <param name="luaBody">lua脚本</param>
        Task<RedisResult> ExecLua(string luaBody);
        #endregion

        #region 发布&订阅
        /// <summary>
        /// 发布
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="channel"></param>
        /// <param name="msg"></param>
        /// <returns></returns>

        Task<long> Publish<T>(string channel, T msg);
        /// <summary>
        /// 订阅
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="onMessageHandler"></param>
        Task Subscribe(string channel, Action<string, string> onMessageHandler = null);
        /// <summary>
        /// 取消订阅
        /// </summary>
        /// <param name="channel"></param>
        Task UnSubscribe(string channel);
        #endregion

        #region 缓存基本操作
        /// <summary>
        /// 获取redis缓存中的值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        Task<T> Get<T>(string key);
        /// <summary>
        /// 获取redis缓存中的值 返回类型 字符串
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        Task<string> Get(string key);
        /// <summary>
        /// 获取redis缓存中多个key下的值
        /// </summary>
        /// <param name="keys"></param>
        /// <returns></returns>
        Task<IDictionary<string, string>> GetAll(IEnumerable<string> keys);
        /// <summary>
        /// 设置redis值 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expiresAt"></param>
        /// <returns></returns>
        Task<bool> Set(string key, string value, DateTime? expiresAt = null);
        /// <summary>
        /// 设置redis值 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expiresAt"></param>
        /// <returns></returns>
        Task<bool> Set<T>(string key, T value, DateTime? expiresAt = null);

        /// <summary>
        /// 设置redis值  如果存在，那么则附加到原来的列表中
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param> 
        /// <returns></returns>
        Task<long> SetAppendTo<T>(string key, T value);
        #endregion

        #region 自增
        /// <summary>
        /// 数字自增
        /// </summary>
        /// <param name="key"></param>
        /// <param name="expiredTime">过期时间</param>
        /// <param name="val"></param>
        Task<long> SetIncr(string key, DateTime? expiredTime, int val = 1);
        #endregion

        #region 自减
        /// <summary>
        /// 数字自减
        /// </summary>
        /// <param name="key"></param>
        /// <param name="expiredTime"></param>
        /// <param name="val"></param>
         Task<long> SetDecr(string key, DateTime? expiredTime, int val = 1);
        #endregion

        #region hash 缓存
        /// <summary>
        /// 
        /// </summary>
        /// <param name="hashId"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expiry"></param>
        Task HSet(string hashId, string key, string value, TimeSpan? expiry = null);
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="hashId"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expiry"></param>
         Task<bool> HSet<T>(string hashId, string key, T value, TimeSpan? expiry = null);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="hashId"></param>
        /// <param name="key"></param>
        /// <returns></returns>
         Task<string> HGet(string hashId, string key);
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="hashId"></param>
        /// <param name="key"></param>
        /// <returns></returns>
         Task<T> HGet<T>(string hashId, string key);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hashId"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
         Task<long> HIncr(string hashId, string key, int value = 1);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="hashId"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
         Task<long> HDecr(string hashId, string key, int value = 1);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="hashId"></param>
        /// <param name="key"></param>
        /// <returns></returns>
         Task<bool> HExists(string hashId, string key);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="hashId"></param>
        /// <param name="key"></param>
        /// <returns></returns>
         Task<bool> HRemove(string hashId, string key);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="hashId"></param>
        /// <returns></returns>
         Task<bool> HRemoveAll(string hashId);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="hashId"></param>
        /// <returns></returns>
         Task<List<string>> HKeys(string hashId);
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="hashId"></param>
        /// <returns></returns>
         Task<List<T>> HValues<T>(string hashId);
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="hashId"></param>
        /// <returns></returns>
         Task<HashSet<T>> HGetAll<T>(string hashId);
        #endregion

        #region set 缓存
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expireIn"></param>
         Task<bool> SAdd(string key, string value, TimeSpan? expireIn = null);
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expireIn"></param>
         Task<bool> SAdd<T>(string key, T value, TimeSpan? expireIn = null);
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="values"></param>
        /// <param name="expireIn"></param>
         Task SAddRange<T>(string key, List<T> values, TimeSpan? expireIn = null);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
         Task<bool> SRem(string key, string value);
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
         Task<bool> SRem<T>(string key, T value);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
         Task<HashSet<string>> SGetAll(string key);
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
         Task<HashSet<T>> SGetAll<T>(string key);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
         Task<long> SCount(string key);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
         Task<bool> SContains(string key, string value);
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
         Task<bool> SContains<T>(string key, T value);
        #endregion

        #region List缓存
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="listId"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        Task<bool> AddItemToList<T>(string listId, T value);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="listId"></param>
        /// <param name="list"></param>

        Task<bool> AddRangeToList(string listId, List<string> list);
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="listId"></param>
        /// <param name="list"></param>
         Task<bool> AddRangeToList<T>(string listId, List<T> list);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="listId"></param>
        /// <returns></returns>
         Task<long> GetListCount(string listId);


        #region 队列操作
        /// <summary>
        /// 入列
        /// </summary>
        /// <param name="listId"></param>
        /// <param name="value"></param>
         Task<long> EnqueueItemOnList(string listId, string value);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="listId"></param>
        /// <returns></returns>
         Task<string> DequeueItemFromList(string listId);
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="listId"></param>
        /// <returns></returns>
         Task<T> DequeueItemFromList<T>(string listId);
        #endregion
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="listId"></param>
        /// <returns></returns>
         Task<List<T>> ListGetAll<T>(string listId);
        #endregion

        #region SortedSet缓存
        /// <summary>
        /// SortedSet缓存
        /// </summary>
        /// <param name="setId"></param>
        /// <returns></returns>
         Task<long> SortedSetCount(string setId);
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="setId"></param>
        /// <param name="value"></param>
        /// <param name="score"></param>
        /// <returns></returns>
         Task<bool> SortedSetAdd<T>(string setId, T value, double? score = null);
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="setId"></param>
        /// <param name="valueList"></param>
        /// <param name="score"></param>
        /// <returns></returns>
         Task<bool> SortedSetAddRange<T>(string setId, List<T> valueList, double? score = null);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="setId"></param>
        /// <param name="value"></param>
        /// <returns></returns>
         Task<bool> SortedSetRemove<T>(string setId, T value);
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="setId"></param>
        /// <returns></returns>
         Task<List<T>> SortedSetMembers<T>(string setId);
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="setId"></param>
        /// <param name="value"></param>
        /// <returns></returns>
         Task<bool> SortedSetContainsItem<T>(string setId, T value);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="setId"></param>
        /// <param name="value"></param>
        /// <returns></returns>
       Task<long> SortedSetItemIndex<T>(string setId, T value);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="set"></param>
        /// <param name="beginRank"></param>
        /// <param name="endRank"></param>
        /// <returns></returns>
        Task<List<string>> GetRangeFromSortedSet(string set, int beginRank, int endRank);
        #endregion
    }
}
