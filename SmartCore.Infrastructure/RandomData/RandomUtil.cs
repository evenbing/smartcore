using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace SmartCore.Infrastructure.RandomData
{
  public static  class RandomUtil
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="size"></param>
        /// <param name="lowerCase"></param>
        /// <returns></returns>
        public static string RandomString(int size, bool lowerCase)
        {
            StringBuilder builder = new StringBuilder();
            Random random = new Random();
            char ch;
            for (int i = 0; i < size; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(ch);
            }
            if (lowerCase)
                return builder.ToString().ToLower();
            return builder.ToString();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="numDown"></param>
        /// <param name="numUp"></param>
        /// <returns></returns>
        public static int GetRandomNum(int numDown, int numUp)
        {     //传递随机数的上下限 用于限制其长度 注意 num_up的值上限1000000000
            int re = 0;
            Random ro = new Random(unchecked((int)DateTime.Now.Ticks));
            re = ro.Next(numDown, numUp);
            return re;
        }

        /// <summary>
        /// Returns an random interger number within a specified rage
        /// </summary>
        /// <param name="min">Minimum number</param>
        /// <param name="max">Maximum number</param>
        /// <returns>Result</returns>
        public static int GetRandomRageNumber(int min = 0, int max = int.MaxValue)
        {
            var randomNumberBuffer = new byte[10];
            new RNGCryptoServiceProvider().GetBytes(randomNumberBuffer);
            return new Random(BitConverter.ToInt32(randomNumberBuffer, 0)).Next(min, max);
        }
        /// <summary>
        /// 使用Guid产生的种子生成真随机数
        /// </summary>
        public static void GetRandomByGuid(int[] array)
        {
            int len = array.Length;
            Random random = new Random(GetRandomSeedbyGuid());
            for (int i = 0; i < len; i++)
            {
                array[i] = random.Next(0, len);
            }
        }
        /// <summary>
        /// 使用Guid生成种子
        /// </summary>
        /// <returns></returns>
        private static int GetRandomSeedbyGuid()
        {
            return new Guid().GetHashCode();
        }
        /// <summary>
        /// 递归，用它来检测生成的随机数是否有重复，如果取出来的数字和已取得的数字有重复就重新随机获取。
        /// </summary>
        /// <param name="arrNum"></param>
        /// <param name="tmp"></param>
        /// <param name="minValue"></param>
        /// <param name="maxValue"></param>
        /// <param name="ra"></param>
        /// <returns></returns>
        public static int GetNum(int[] arrNum, int tmp, int minValue, int maxValue, Random ra)
        {
            int n = 0;
            while (n <= arrNum.Length - 1)
            {
                if (arrNum[n] == tmp) //利用循环判断是否有重复
                {
                    tmp = ra.Next(minValue, maxValue); //重新随机获取。
                    GetNum(arrNum, tmp, minValue, maxValue, ra);//递归:如果取出来的数字和已取得的数字有重复就重新随机获取。
                }
                n++;
            }
            return tmp;
        }
        /// <summary>
        /// 生成一个固定位数由数字组成的随机字符串
        /// </summary>
        /// <param name="Length"></param>
        /// <returns></returns>
        public static string GenerateRandomNumber(int Length)
        {
            StringBuilder newRandom = new StringBuilder(Length);
            Random rd = new Random();
            char[] constant = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
            for (int i = 0; i < Length; i++)
            {
                newRandom.Append(constant[rd.Next(Length)]);
            }
            return newRandom.ToString();
        }

         
             
    }
}
