using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimCaptcha.Models.Slider
{
    public class SliderVCodeResponseModel : VCodeResponseModel
    {
        public class DataModel
        {
            /// <summary>
            /// click
            /// slider
            /// </summary>
            public string captchaType { get; set; } = "slider";

            /// <summary>
            /// 用户此次会话唯一标识
            /// </summary>
            public string userId { get; set; }

            /// <summary>
            /// 验证背景 图片base64
            /// </summary>
            public string bgImg { get; set; }

            /// <summary>
            /// 验证滑块 图片base64
            /// </summary>
            public string sliderImg { get; set; }

            /// <summary>
            /// 验证提示
            /// </summary>
            public string vCodeTip { get; set; }


        }
    }
}
