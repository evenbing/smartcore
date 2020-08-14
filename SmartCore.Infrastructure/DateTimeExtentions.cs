using Microsoft.Extensions.Internal;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartCore.Infrastructure
{
   
    public static class DateTimeExtentions
    {
        /// <summary>
        /// 将客户端时间转换为服务端本地时间
        /// </summary>
        /// <param name="clientTime">客户端时间</param>
        /// <returns>返回服务端本地时间</returns>
        public static DateTime ToLocalServerTime(this DateTime clientTime)
        { 
            DateTime serverTime = TimeZoneInfo.ConvertTime(clientTime, TimeZoneInfo.Local);
            return serverTime;
        }
    }
}
