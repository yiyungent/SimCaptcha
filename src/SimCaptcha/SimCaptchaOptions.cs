using System;
using System.Collections.Generic;
using System.Text;

namespace SimCaptcha
{
    public class SimCaptchaOptions
    {
        public const string SimCaptcha = "SimCaptcha";

        public string AppId { get; set; }
        public string AesKey { get; set; }

        /// <summary>
        /// 允许的错误次数，例如允许2次，那么错误2次后，还可以再次尝试check，但第3次错误后就不能再尝试了
        /// </summary>
        public int ErrorNum { get; set; }

        /// <summary>
        /// 验证码在被创建多少秒后过期
        /// </summary>
        public int ExpiredSec { get; set; }

    }

}
