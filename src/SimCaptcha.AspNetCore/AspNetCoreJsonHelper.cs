using System.Text.Json;
using SimCaptcha.Interface;
// Project: SimCaptcha
// https://github.com/yiyungent/SimCaptcha
// Author: yiyun <yiyungent@gmail.com>

namespace SimCaptcha.AspNetCore
{
    public class AspNetCoreJsonHelper : IJsonHelper
    {
        public JsonSerializerOptions JsonSerializerOptions { get; set; }

        public AspNetCoreJsonHelper()
        {
            this.JsonSerializerOptions = new JsonSerializerOptions();
            // 属性名大小写不敏感
            this.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
        }

        public T Deserialize<T>(string jsonStr)
        {
            return System.Text.Json.JsonSerializer.Deserialize<T>(jsonStr, JsonSerializerOptions);
        }

        public string Serialize(object jsonObj)
        {
            return System.Text.Json.JsonSerializer.Serialize(jsonObj);
        }
    }
}
