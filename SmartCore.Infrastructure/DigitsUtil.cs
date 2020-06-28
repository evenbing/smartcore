using System;
using System.Collections.Generic;
using System.Text;

namespace SmartCore.Infrastructure
{
    /// <summary>
    /// 进制相互转换
    /// </summary>
   public static class DigitsUtil
    {
        #region 将10进制数字转成62进制数字
        static char[] DIGITS =
{ '0', '1', '2', '3', '4', '5', '6', '7', '8', '9',
'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j',
'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't',
'u', 'v', 'w', 'x', 'y', 'z', 'A', 'B', 'C', 'D',
'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N',
'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' };
        private static int EXPONENT = DIGITS.Length;//幂数

        #endregion
        /// <summary>
        /// 
        /// </summary>
        /// <param name="seq"></param>
        /// <returns></returns>
        public static string ConvertTo62RadixString(int seq)
        {
            StringBuilder sBuilder = new StringBuilder();
            while (true)
            {
                int remainder =seq % EXPONENT;
                sBuilder.Append(DIGITS[remainder]);
                seq = seq / EXPONENT;
                if (seq == 0)
                {
                    break;
                }
            }
            char[] result = sBuilder.ToString().ToCharArray();
            Array.Reverse(result);
            return new string(result);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static int RadixString(string str)
        {
            char[] result = str.ToString().ToCharArray();
            Array.Reverse(result);
            string reversedString = new string(result);
            int sum = 0;
            int len = reversedString.Length;
            for (int i = 0; i < len; i++)
            { 
                sum += (int)(IndexDigits(str[len - i - 1]) * Math.Pow(EXPONENT, i));

            }
            return sum;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ch"></param>
        /// <returns></returns>
        private static double IndexDigits(char ch)
        {
            for (int i = 0; i < DIGITS.Length; i++)
            {
                if (ch == DIGITS[i])
                {
                    return i;
                }
            }
            return -1;
        }
    }
}
