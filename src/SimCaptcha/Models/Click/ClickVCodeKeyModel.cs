using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimCaptcha.Models.Click
{
    public class ClickVCodeKeyModel : VCodeKeyModel
    {
        /// <summary>
        /// 正确的 验证码点触位置数据
        /// <para>用于效验 用户点触位置</para>
        /// </summary>
        public IList<PointPosModel> VCodePos { get; set; }
    }
}
