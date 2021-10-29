﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimCaptcha.ResponseModels.Click
{
    public class ClickVCodeResponseModel : VCodeResponseModel
    {
        public class DataModel
        {
            /// <summary>
            /// click
            /// slider
            /// </summary>
            public string captchaType { get; set; } = "click";

            /// <summary>
            /// 用户此次会话唯一标识
            /// </summary>
            public string userId { get; set; }

            /// <summary>
            /// 验证图片base64
            /// </summary>
            public string vCodeImg { get; set; }

            /// <summary>
            /// 验证提示
            /// </summary>
            public string vCodeTip { get; set; }

            /// <summary>
            /// 答案: 字(有顺序 eg: 望,我,哈,他),  也可以为空, 目前前端只用vCodeTip,无用,保留 
            /// </summary>
            public IList<string> words { get; set; }
        }
    }
}