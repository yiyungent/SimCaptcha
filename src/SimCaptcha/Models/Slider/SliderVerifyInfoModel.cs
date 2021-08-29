using System;
using System.Collections.Generic;
using System.Text;

namespace SimCaptcha.Models.Slider
{
    public class SliderVerifyInfoModel : VerifyInfoModel
    {
        /// <summary>
        /// 验证码 最终滑动位置数据
        /// <para>用于效验 用户滑动位置</para>
        /// </summary>
        public PointPosModel VCodePos { get; set; }

        /// <summary>
        /// 滑动轨迹
        /// </summary>
        public List<PointPosModel> TrackPoints { get; set; }
    }
}
