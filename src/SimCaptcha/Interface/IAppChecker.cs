using SimCaptcha.Models;
// Project: SimCaptcha
// https://github.com/yiyungent/SimCaptcha
// Author: yiyun <yiyungent@gmail.com>

namespace SimCaptcha.Interface
{
    public interface IAppChecker
    {
        ISimCaptchaOptions Options { get; set; }

        /// <summary>
        /// 效验 appId, appSecret 是否有效
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="appSecret"></param>
        /// <returns><see cref="bool"/>是否有效 <see cref="string"/>错误消息</returns>
        AppCheckModel Check(string appId, string appSecret);

        /// <summary>
        /// 效验 appId 是否有效
        /// </summary>
        /// <param name="appId"></param>
        /// <returns><see cref="bool"/>是否有效 <see cref="string"/>错误消息</returns>
        AppCheckModel CheckAppId(string appId);
    }
}
