using System;
using System.Collections.Generic;
using System.Text;

namespace SimCaptcha.Models
{
    /// <summary>
    /// verifyInfo = { vcodePos: vCodePos, vCodeKey: vCodeKey, ua: navigator.userAgent, ts: ts }
    /// </summary>
    public class VerifyInfoModel
    {
        public IList<PointPosModel> VCodePos { get; set; }

        public string VCodeKey { get; set; }

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
