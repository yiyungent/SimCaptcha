using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimCaptcha.Models.Slider
{
    public class SliderVCodeImgModel : VCodeImgModel
    {
        /// <summary>
        /// 验证背景 图片base64
        /// </summary>
        public string BgImg { get; set; }

        /// <summary>
        /// 验证滑块 图片base64
        /// </summary>
        public string SliderImg { get; set; }

        /// <summary>
        /// 答案: 滑块位置数据
        /// 滑块左上角位置
        /// </summary>
        public (int X, int Y) VCodePos { get; set; }
    }
}
