using SimCaptcha.Common;
using SimCaptcha.Interface;
using SimCaptcha.Models;
using SimCaptcha.ResponseModels;
using System;
// Project: SimCaptcha
// https://github.com/yiyungent/SimCaptcha
// Author: yiyun <yiyungent@gmail.com>

namespace SimCaptcha
{
    /// <summary>
    /// 验证码客户端
    /// </summary>
    public class SimCaptchaClient
    {
        #region Fields
        private ISimCaptchaOptions _options;

        /// <summary>
        /// 错误日志记录(非必需,可能为null)
        /// </summary>
        private ILogHelper _logHelper;
        #endregion

        #region Properties
        public IJsonHelper JsonHelper { get; set; }
        #endregion

        #region Ctor
        /// <summary>
        /// 
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="appSecret"></param>
        /// <param name="verifyUrl">验证码服务端提供的验证ticket的url</param>
        public SimCaptchaClient(ISimCaptchaOptions options, IJsonHelper jsonHelper, ILogHelper logHelper)
        {
            this._options = options;
            this.JsonHelper = jsonHelper;
            this._logHelper = logHelper;
        }
        #endregion

        #region Set
        public SimCaptchaClient Set(ISimCaptchaOptions options)
        {
            this._options = options;
            return this;
        }

        public SimCaptchaClient Set(IJsonHelper jsonHelper)
        {
            this.JsonHelper = jsonHelper;
            return this;
        }

        public SimCaptchaClient Set(ILogHelper logHelper)
        {
            this._logHelper = logHelper;
            return this;
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
            //string reqStr = $"appId={_options.AppId}&appSecret={_options.AppSecret}&ticket={ticket}&userId={userId}&userIp={userIp}";
            // 效验票据
            try
            {
                string[] headers = { "Content-Type: application/json" };
                //string[] headers = { "Content-Type: application/x-www-form-urlencoded" };
                string resJsonStr = HttpAide.HttpPost(_options.TicketVerifyUrl, postDataStr: reqJsonStr, headers: headers);
                //string resJsonStr = HttpAide.HttpPost(_options.TicketVerifyUrl, postDataStr: reqStr);
                ticketVerifyModel = JsonHelper.Deserialize<TicketVerifyResponseModel>(resJsonStr);
            }
            catch (Exception ex)
            {
                this._logHelper?.Write(ex.ToString());
            }

            return ticketVerifyModel;
        }
        #endregion

    }
}
