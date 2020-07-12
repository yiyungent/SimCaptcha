using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SimCaptcha;
using SimCaptcha.AspNetCore;

namespace AspNetCoreClient.Controllers
{
    /// <summary>
    /// 后台业务
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class HomeController : ControllerBase
    {
        private readonly IHttpContextAccessor _accessor;

        /// <summary>
        /// 验证码配置信息
        /// </summary>
        private readonly SimCaptchaOptions _options;

        private readonly SimCaptchaClient _client;

        #region Ctor
        public HomeController(IOptions<SimCaptchaOptions> options, IHttpContextAccessor accessor)
        {
            _options = options.Value;
            this._client = new SimCaptchaClient(options.Value, new AspNetCoreJsonHelper());

            this._accessor = accessor;
        }
        #endregion

        #region 登录
        /// <summary>
        /// 登陆
        /// </summary>
        /// <returns></returns>
        [Route(nameof(Login))]
        [HttpPost]
        public ActionResult Login(string userName, string password, string ticket, string userId)
        {
            string userIp = _accessor.HttpContext.Connection.RemoteIpAddress.ToString();
            SimCaptcha.Models.TicketVerifyResponseModel ticketVerifyResult = _client.Verify(ticket, userId, userIp);
            if (ticketVerifyResult.code != 0)
            {
                return Ok(ticketVerifyResult);
            }

            if (userName == "admin" && password == "admin")
            {
                return Ok(new { code = 1, message = "登录成功" });
            }
            else
            {
                return Ok(new { code = -10, message = "账号密码错误!" });
            }
        } 
        #endregion
    }
}
