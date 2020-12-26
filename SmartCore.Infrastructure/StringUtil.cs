using System;
using System.Collections.Generic;
using System.Text;

namespace SmartCore.Infrastructure
{
    /// <summary>
    /// 
    /// </summary>
    public class StringUtil
    {
        /// <summary>
        /// 将传入的字符串中间部分字符替换成特殊字符
        /// </summary>
        /// <param name="value">需要替换的字符串</param>
        /// <param name="startLen">前保留长度</param>
        /// <param name="endLen">尾保留长度</param>
        /// <param name="replaceChar">特殊字符</param>
        /// <returns>被特殊字符替换的字符串</returns>
        public static string ReplaceWithSpecialChar(string value, int startLen = 4, int endLen = 4, char specialChar = '*')
        {
            try
            {
                int lenth = value.Length - startLen - endLen;

                string replaceStr = value.Substring(startLen, lenth);

                string specialStr = string.Empty;

                for (int i = 0; i < replaceStr.Length; i++)
                {
                    specialStr += specialChar;
                }

                value = value.Replace(replaceStr, specialStr);
            }
            catch (Exception)
            {
                throw;
            }

            return value;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string HashByteToString(byte[] data)
        {
            StringBuilder sBuilder = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("X2"));
            }
            return sBuilder.ToString();
        }
        #region 让单词首字母大写
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetTitleCaseLetter(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return "";
            }
            return System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(value);
        }
        #endregion
        /// <summary>
        /// 翻转一个字符串
        /// </summary>
        /// <param name="inputString">目标字符串</param>
        /// <returns>翻转后的字符串</returns>
        public static string ReverseStr(string inputString)
        {
            char[] c = inputString.ToCharArray();
            System.Array.Reverse(c);
            return new string(c);
        }

    }
}
