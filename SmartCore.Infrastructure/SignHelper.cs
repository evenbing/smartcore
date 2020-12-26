using SmartCore.Infrastructure.Security;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartCore.Infrastructure
{
    /// <summary>
    /// 
    /// </summary>
    public class SignHelper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sortedDictionary"></param>
        /// <param name="host"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        public static string CreateSignForApi(SortedDictionary<string, string> sortedDictionary, string host = "", string method = "")
        {
            StringBuilder tempSign = new StringBuilder();
            foreach (KeyValuePair<string, string> keyValues in sortedDictionary)
            {
                if (!string.IsNullOrEmpty(keyValues.Value))
                {
                    tempSign.AppendFormat("{0}={1}&", keyValues.Key.ToLower(), keyValues.Value);
                }
            }
            // string encodeValue = Encode.UrlEncode(tempSign.ToString().TrimEnd('&'), System.Text.Encoding.UTF8);
            if (!string.IsNullOrEmpty(host))
            {
                 tempSign.Insert(0, host);
            }
            if (!string.IsNullOrEmpty(method))
            {
                tempSign.Append(method);
            }
            return Md5Util.Md5Hash(tempSign.ToString());
        }
    }
}
