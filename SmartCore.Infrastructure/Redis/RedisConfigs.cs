using System;
using System.Collections.Generic;

namespace SmartCore.Infrastructure.Redis
{
   public class RedisConfig
    {
        /// <summary>
        /// 
        /// </summary>
        public bool IsCluster { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int DefaultDb { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int SyncTimeout { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int ConnectTimeout { get; set; }

        public List<string> Hosts { get; set; }
    }
}
