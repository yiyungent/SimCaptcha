using SimCaptcha.Interface;
using System.Collections.Generic;
// Project: SimCaptcha
// https://github.com/yiyungent/SimCaptcha
// Author: yiyun <yiyungent@gmail.com>

namespace SimCaptcha
{
    public class SimCaptchaOptions : ISimCaptchaOptions
    {
        public const string SimCaptcha = "SimCaptcha";

        public string AppId { get; set; }

        public string AppSecret { get; set; }

        public string TicketVerifyUrl { get; set; }


        // ↑以上仅供业务后台使用
        // ↓以下仅供验证码服务端使用


        public string EncryptKey { get; set; }

        /// <summary>
        /// 允许的错误次数，例如允许2次，那么错误2次后，还可以再次尝试check，但第3次错误后就不能再尝试了
        /// </summary>
        public int AllowErrorNum { get; set; }

        /// <summary>
        /// <para>验证码在被创建多少秒后过期</para>
        /// <para>ticket在被创建多少秒后过期</para>
        /// <para>均使用此属性</para>
        /// </summary>
        public int ExpiredSec { get; set; }

        public IList<AppItemModel> AppList { get; set; }

        #region Ctor
        public SimCaptchaOptions()
        {
            // 初始默认值
            this.AllowErrorNum = 1;
            this.ExpiredSec = 60;
            this.AppList = new List<AppItemModel>();
        }
        #endregion

    }

    public class AppItemModel
    {
        public string AppId { get; set; }
        public string AppSecret { get; set; }

        /// <summary>
        /// 允许跨域的白名单
        /// </summary>
        public List<string> CorsWhiteList { get; set; }
    }

}
