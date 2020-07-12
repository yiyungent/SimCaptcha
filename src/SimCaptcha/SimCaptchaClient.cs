using SimCaptcha.Common;
using SimCaptcha.Interface;
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
        private SimCaptchaOptions _options;

        public IJsonHelper JsonHelper { get; set; }

        #region Ctor
        /// <summary>
        /// 
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="appSecret"></param>
        /// <param name="verifyUrl">验证码服务端提供的验证ticket的url</param>
        public SimCaptchaClient(SimCaptchaOptions options, IJsonHelper jsonHelper)
        {
            this._options = options;
            this.JsonHelper = jsonHelper;
        }
        #endregion

        #region 效验票据有效性
        /// <summary>
        /// 效验票据有效性
        /// </summary>
        /// <param name="ticket">验证码客户端验证回调的票据</param>
        /// <param name="userId">用户会话唯一标识</param>
        /// <param name="userIp">提交验证的用户的IP地址（eg: 10.127.10.2）</param>
        /// <returns></returns>
        public TicketVerifyResponseModel Verify(string ticket, string userId, string userIp)
        {
            TicketVerifyResponseModel ticketVerifyModel = new TicketVerifyResponseModel { code = -1, message = "效验失败" };
            string reqJsonStr = JsonHelper.Serialize(new { appId = _options.AppId, appSecret = _options.AppSecret, ticket, userId, userIp });
            // 效验票据
            try
            {
                string resJsonStr = HttpAide.HttpPost(_options.TicketVerifyUrl, postDataStr: reqJsonStr);
                ticketVerifyModel = JsonHelper.Deserialize<TicketVerifyResponseModel>(resJsonStr);
            }
            catch (Exception ex)
            { }

            return ticketVerifyModel;
        }
        #endregion

    }
}
