using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimCaptcha.Models.Click
{
    public class ClickVCodeImgModel : VCodeImgModel
    {
        /// <summary>
        /// 图片的base64 字符串
        /// </summary>
        public string ImgBase64 { get; set; }

        /// <summary>
        /// 答案: 字 (有顺序) eg: 望,我,哈,他
        /// <para>可以为null,目前前端只用到了 VCodeTip</para>
        /// </summary>
        public IList<string> Words { get; set; }

        /// <summary>
        /// 答案: 点触位置数据
        /// </summary>
        public IList<PointPosModel> VCodePos { get; set; }
    }
}
