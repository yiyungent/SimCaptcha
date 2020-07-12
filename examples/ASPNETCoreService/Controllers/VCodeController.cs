using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using SimCaptcha;
using SimCaptcha.AspNetCore;
using SimCaptcha.Common;
using SimCaptcha.Extensions;
using SimCaptcha.Interface;
using SimCaptcha.Models;

namespace AspNetCoreService.Controllers
{
    /// <summary>
    /// 验证码服务端
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class VCodeController : ControllerBase
    {
        private readonly IHttpContextAccessor _accessor;

        /// <summary>
        /// 验证码配置信息
        /// </summary>
        private readonly SimCaptchaOptions _options;

        private readonly SimCaptchaService _service;

        private readonly IAppChecker _appChecker;

        #region Ctor
        public VCodeController(
            IOptions<SimCaptchaOptions> options,
            IHttpContextAccessor accessor,
            IMemoryCache memoryCache)
        {
            _options = options.Value;
            this._service = new SimCaptchaService(
                new LocalCache(memoryCache) { TimeOut = options.Value.ExpiredSec },
                new AspNetCoreVCodeImage(),
                new AspNetCoreJsonHelper(),
                options.Value);
            this._appChecker = new DefaultAppChecker(options.Value);

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
            // appId 效验: 这通常需要你自己根据业务实现 IAppChecker
            (bool, string) appCheckResult = _appChecker.CheckAppId(verifyInfo.AppId);
            if (!appCheckResult.Item1)
            {
                // -6 appId 效验不通过 -> 不允许验证, 提示错误信息
                responseModel = new VCodeCheckResponseModel { code = -6, message = appCheckResult.Item2 };
                return Ok(responseModel);
            }
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
        public IActionResult TicketVerify(string appId, string appSecret, string ticket, string userId, string userIp)
        {
            TicketVerifyResponseModel responseModel = null;
            // appId, appSecret效验: 这通常需要你自己根据业务实现 IAppChecker
            (bool, string) appCheckResult = _appChecker.Check(appId, appSecret);
            if (!appCheckResult.Item1)
            {
                // -7 AppId,AppSecret效验不通过
                responseModel = new TicketVerifyResponseModel { code = -7, message = appCheckResult.Item2 };
                return Ok(responseModel);
            }

            // ticket 效验
            responseModel = _service.TicketVerify(appId, appSecret, ticket, userId, userIp);

            return Ok(responseModel);
        }
        #endregion

    }
}
