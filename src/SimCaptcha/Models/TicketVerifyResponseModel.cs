using System;
using System.Collections.Generic;
using System.Text;

namespace SimCaptcha.Models
{
    public class TicketVerifyResponseModel
    {
        /// <summary>
        /// <para>0 OK. 验证通过</para>
        /// <para>-1 captcha no match 验证不通过</para>
        /// <para>-2 verify ticket timeout. 验证码ticket过期</para>
        /// <para>-3 verify ip no match. ip不匹配</para>
        /// <para>-4 decrypt fail. 验证码ticket解密失败</para>
        /// <para>-5 验证码ticket无效, 可能已被使用过一次以至于被删除,或则过期被自动删除</para>
        /// <para>-6 验证码ticket无效, 篡改ticket, 与验证码服务端保存的此会话ticket不一致</para>
        /// <para>-7 AppId,AppSecret效验不通过</para>
        /// </summary>
        public int code { get; set; }

        public string message { get; set; }
    }
}
