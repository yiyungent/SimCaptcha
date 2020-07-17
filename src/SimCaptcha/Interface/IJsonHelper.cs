// Project: SimCaptcha
// https://github.com/yiyungent/SimCaptcha
// Author: yiyun <yiyungent@gmail.com>

namespace SimCaptcha.Interface
{
    public interface IJsonHelper
    {
        /// <summary>
        /// 将对象转化为json字符串
        /// </summary>
        /// <param name="jsonObj">对象</param>
        string Serialize(object jsonObj);

        /// <summary>
        /// 将json字符串还原为目标对象
        /// </summary>
        /// <param name="jsonStr">json字符串</param>
        T Deserialize<T>(string jsonStr);
    }
}
