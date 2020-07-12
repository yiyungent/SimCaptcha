using SimCaptcha.Interface;
using System;
using System.Collections.Generic;
using System.Text;

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
