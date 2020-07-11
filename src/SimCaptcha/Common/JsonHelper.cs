using System;
using System.Collections.Generic;
using System.Text;

namespace SimCaptcha.Common
{
    public class JsonHelper
    {
        /// <summary>
        /// 将对象转化为json字符串
        /// </summary>
        /// <param name="jsonObj">对象</param>
        public static string Serialize(object jsonObj)
        {
            return System.Text.Json.JsonSerializer.Serialize(jsonObj);
        }

        /// <summary>
        /// 将json字符串还原为目标对象
        /// </summary>
        /// <param name="jsonStr">json字符串</param>
        public static T Deserialize<T>(string jsonStr)
        {
            return System.Text.Json.JsonSerializer.Deserialize<T>(jsonStr);
        }
    }
}
