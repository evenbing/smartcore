using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartCore.Infrastructure.Json
{
    public class JsonHelper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="objValue"></param>
        /// <returns></returns>
        public static string SerializeObject(object objValue)
        {
            if (objValue == null)
                throw new ArgumentNullException("value");

            return JsonConvert.SerializeObject(objValue);
        }
    }
}
