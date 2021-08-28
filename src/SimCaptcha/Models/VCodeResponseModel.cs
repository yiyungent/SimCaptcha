using System.Collections.Generic;
// Project: SimCaptcha
// https://github.com/yiyungent/SimCaptcha
// Author: yiyun <yiyungent@gmail.com>

namespace SimCaptcha.Models
{
    public class VCodeResponseModel
    {
        /// <summary>
        /// 0 成功
        /// </summary>
        public int code { get; set; }

        public string message { get; set; }

        public object data { get; set; }

    }
}
