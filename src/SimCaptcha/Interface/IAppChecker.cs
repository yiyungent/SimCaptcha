using System;
using System.Collections.Generic;
using System.Text;

namespace SimCaptcha.Interface
{
    public interface IAppChecker
    {
        public SimCaptchaOptions Options { get; set; }

        /// <summary>
        /// 效验 appId, appSecret 是否有效
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="appSecret"></param>
        /// <returns><see cref="bool"/>是否有效 <see cref="string"/>错误消息</returns>
        public (bool, string) Check(string appId, string appSecret);

        /// <summary>
        /// 效验 appId 是否有效
        /// </summary>
        /// <param name="appId"></param>
        /// <returns><see cref="bool"/>是否有效 <see cref="string"/>错误消息</returns>
        public (bool, string) CheckAppId(string appId);
    }
}
