using System.Collections.Generic;
// Project: SimCaptcha
// https://github.com/yiyungent/SimCaptcha
// Author: yiyun <yiyungent@gmail.com>

namespace SimCaptcha.Models
{
    /// <summary>
    /// 存储正确的点触验证位置数据等
    /// </summary>
    public class VCodeKeyModel
    {
        /// <summary>
        /// 验证码创建时间
        /// <para>js 13位 毫秒时间戳</para>
        /// </summary>
        public long TS { get; set; }

        /// <summary>
        /// 已经错误次数
        /// </summary>
        public int ErrorNum { get; set; }
    }
}
