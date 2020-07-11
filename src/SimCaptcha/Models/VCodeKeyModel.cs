using System;
using System.Collections.Generic;
using System.Text;

namespace SimCaptcha.Models
{
    /// <summary>
    /// 存储正确的点触验证位置数据等
    /// </summary>
    public class VCodeKeyModel
    {
        /// <summary>
        /// 正确的 验证码点触位置数据
        /// <para>用于效验 用户点触位置</para>
        /// </summary>
        public IList<PointPosModel> VCodePos { get; set; }

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
