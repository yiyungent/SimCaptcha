using System;
using System.Collections.Generic;
using System.Text;

namespace SimCaptcha.Models.Slider
{
    public class SliderVCodeKeyModel : VCodeKeyModel
    {
        /// <summary>
        /// 正确的 验证码点触位置数据
        /// <para>用于效验 用户滑动位置</para>
        /// </summary>
        public (int X, int Y) VCodePos { get; set; }
    }
}
