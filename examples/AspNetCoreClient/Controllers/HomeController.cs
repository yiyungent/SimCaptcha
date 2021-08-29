using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SimCaptcha;
using SimCaptcha.AspNetCore;
using SimCaptcha.AspNetCore.Implement;
using SimCaptcha.ResponseModels;

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
            this._client = new SimCaptchaClient(options.Value, new AspNetCoreJsonHelper(), new ConsoleLogHelper());

            this._accessor = accessor;
        }
        #endregion

        #region 登录
        /// <summary>
        /// 登陆
        /// <para>注意:标记<see cref="FromFormAttribute"/>才能接收到</para>
        /// </summary>
        /// <returns></returns>
        [Route(nameof(Login))]
        [HttpPost]
        public ActionResult Login([FromForm] string userName, [FromForm] string password, [FromForm] string ticket, [FromForm] string userId)
        {
            if (string.IsNullOrEmpty(ticket) || string.IsNullOrEmpty(userId))
            {
                return Ok(new { code = -11, message = "请点击验证" });
            }
            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
            {
                return Ok(new { code = -12, message = "请输入账号,密码" });
            }

            string userIp = _accessor.HttpContext.Connection.RemoteIpAddress.ToString();
            TicketVerifyResponseModel ticketVerifyResult = _client.Verify(ticket, userId, userIp);
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
