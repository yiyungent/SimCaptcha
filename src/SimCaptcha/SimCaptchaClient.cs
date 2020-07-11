using SimCaptcha.Common;
using SimCaptcha.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimCaptcha
{
    /// <summary>
    /// 验证码客户端
    /// </summary>
    public class SimCaptchaClient
    {
        public string AppId { get; set; }

        public string AppSecret { get; set; }

        public string VerifyUrl { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="appSecret"></param>
        /// <param name="verifyUrl">验证码服务端提供的验证ticket的url</param>
        public SimCaptchaClient(string appId, string appSecret, string verifyUrl)
        {
            this.AppId = appId;
            this.AppSecret = appSecret;
            this.VerifyUrl = verifyUrl;
        }

        /// <summary>
        /// 效验票据有效性
        /// </summary>
        /// <param name="ticket">验证码客户端验证回调的票据</param>
        /// <param name="userId">用户会话唯一标识</param>
        /// <param name="userIP">提交验证的用户的IP地址（eg: 10.127.10.2）</param>
        /// <returns></returns>
        public TicketVerifyResponseModel Verify(string ticket, string userId, string userIP)
        {
            TicketVerifyResponseModel ticketVerifyModel = new TicketVerifyResponseModel { code = -1, message = "效验失败" };
            string reqJsonStr = JsonHelper.Serialize(new { appId = AppId, appSecret = AppSecret, ticket, userId, userIP });
            // 效验票据
            try
            {
                string resJsonStr = HttpAide.HttpPost(this.VerifyUrl, postDataStr: reqJsonStr);
                ticketVerifyModel = JsonHelper.Deserialize<TicketVerifyResponseModel>(resJsonStr);
            }
            catch (Exception ex)
            { }

            return ticketVerifyModel;
        }
    }
}
