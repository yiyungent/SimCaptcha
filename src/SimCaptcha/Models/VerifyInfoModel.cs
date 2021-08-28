using System.Collections.Generic;
// Project: SimCaptcha
// https://github.com/yiyungent/SimCaptcha
// Author: yiyun <yiyungent@gmail.com>

namespace SimCaptcha.Models
{
    /// <summary>
    /// verifyInfo = { vcodePos: vCodePos, ua: navigator.userAgent, ts: ts }
    /// </summary>
    public class VerifyInfoModel
    {
        public string AppId { get; set; }

        /// <summary>
        /// 用户会话唯一标识
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// 未用，保留
        /// </summary>
        public string UA { get; set; }

        /// <summary>
        /// js 13位 毫秒时间戳
        /// 未用，保留
        /// </summary>
        public long TS { get; set; }
    }

}
