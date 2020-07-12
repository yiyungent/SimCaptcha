using System;
using System.Collections.Generic;
using System.Text;

namespace SimCaptcha.Models
{
    public class VCodeImgModel
    {
        /// <summary>
        /// 图片的base64 字符串
        /// </summary>
        public string ImgBase64 { get; set; }

        /// <summary>
        /// 验证提示: eg: 请依次点击 望,我,哈,他
        /// </summary>
        public string VCodeTip { get; set; }



        /// <summary>
        /// 答案: 字 (有顺序) eg: 望,我,哈,他
        /// </summary>
        public IList<string> Words { get; set; }

        /// <summary>
        /// 答案: 点触位置数据
        /// </summary>
        public IList<PointPosModel> VCodePos { get; set; }
    }
}
