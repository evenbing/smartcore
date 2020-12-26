using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace SmartCore.Infrastructure.Security
{
    /// <summary>
    /// 
    /// </summary>
    public class Md5Util
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="toUpper"></param>
        /// <returns></returns>
        public static string Md5Hash(string source,bool toUpper=true)
        {
            using (var md5Hash = MD5.Create())
            {
                byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(source));
                StringBuilder sBuilder = new StringBuilder();
                for (int i = 0; i < data.Length; i++)
                {
                    sBuilder.Append(data[i].ToString(toUpper?"X2":"x2"));
                }
                return sBuilder.ToString();
            } 
        }

    }
}
