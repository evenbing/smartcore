using System;
using System.Collections.Generic;
using System.Text;

namespace SmartCore.Models.DTO
{
    public class JwtAuthorizationDTO
    {
        /// <summary>
        /// 
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string token_type { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string access_token { get; set; }
        /// <summary>
        /// 
        /// </summary>

        public string refresh_token { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public long access_token_expired { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public long refresh_token_expired { get; set; }
    } 
}
