using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using SimCaptcha;
using SimCaptcha.AspNetCore;
using SimCaptcha.Models;

namespace AspNetCoreService.Controllers
{
    /// <summary>
    /// 验证码服务端
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("_VCodeAllowSpecificOrigins")]
    // TODO: 目前未做 CorsWhiteList 对于每一个App 的严格限制(使用此AppId的只能是对应的CorsWhiteList)
    public class VCodeController : ControllerBase
    {
        private readonly IHttpContextAccessor _accessor;

        /// <summary>
        /// 验证码配置信息
        /// </summary>
        private readonly SimCaptchaOptions _options;

        private readonly SimCaptchaService _service;

        #region Ctor
        public VCodeController(
            IOptions<SimCaptchaOptions> options,
            IHttpContextAccessor accessor,
            IMemoryCache memoryCache)
        {
            _options = options.Value;
            this._service = new SimCaptchaService(
                options.Value,
                new LocalCache(memoryCache) { TimeOut = options.Value.ExpiredSec },
                new AspNetCoreVCodeImage(),
                new AspNetCoreJsonHelper(),
                new ConsoleLogHelper()
                );

            this._accessor = accessor;
        }
        #endregion

        #region 获取验证码
        /// <summary>
        /// 获取验证码 - 配置好验证码服务端的SimCaptcha.js后, 由SimCaptcha.js自动处理(无需业务后台关注)
        /// </summary>
        /// <returns></returns>
        [Route(nameof(VCodeImg))]
        [HttpGet]
        public async Task<IActionResult> VCodeImg()
        {
            VCodeResponseModel rtnResult = await _service.VCode();

            return Ok(rtnResult);
        }
        #endregion

        #region 验证码效验
        /// <summary>
        /// 效验验证码 - 配置好验证码服务端的SimCaptcha.js后, 由SimCaptcha.js自动处理(往返于用户浏览器与验证码服务端，无需业务后台关注)
        /// </summary>
        /// <param name="verifyInfo"></param>
        /// <returns></returns>
        [Route(nameof(VCodeCheck))]
        [HttpPost]
        public IActionResult VCodeCheck(VerifyInfoModel verifyInfo)
        {
            VCodeCheckResponseModel responseModel = null;

            // 获取ip地址
            string userIp = _accessor.HttpContext.Connection.RemoteIpAddress.ToString();
            responseModel = _service.VCodeCheck(verifyInfo, userIp);

            return Ok(responseModel);
        }
        #endregion

        #region ticket效验
        /// <summary>
        /// ticket效验 - 给业务后台验证使用
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="appSecret"></param>
        /// <param name="ticket"></param>
        /// <param name="userId"></param>
        /// <param name="userIp"></param>
        /// <returns></returns>
        [Route(nameof(TicketVerify))]
        [HttpPost]
        public IActionResult TicketVerify(TicketVerifyModel ticketVerify)
        {
            TicketVerifyResponseModel responseModel = null;

            // ticket 效验
            responseModel = _service.TicketVerify(ticketVerify.AppId, ticketVerify.AppSecret, ticketVerify.Ticket, ticketVerify.UserId, ticketVerify.UserIp);

            return Ok(responseModel);
        }
        #endregion

    }
}
