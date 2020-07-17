using SimCaptcha.Interface;
// Project: SimCaptcha
// https://github.com/yiyungent/SimCaptcha
// Author: yiyun <yiyungent@gmail.com>

namespace SimCaptcha.AspNetCore
{
    public class AspNetCoreJsonHelper : IJsonHelper
    {
        public T Deserialize<T>(string jsonStr)
        {
            return System.Text.Json.JsonSerializer.Deserialize<T>(jsonStr);
        }

        public string Serialize(object jsonObj)
        {
            return System.Text.Json.JsonSerializer.Serialize(jsonObj);
        }
    }
}
