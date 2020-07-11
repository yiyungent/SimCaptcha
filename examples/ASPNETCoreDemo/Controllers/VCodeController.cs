using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using ASPNETCoreDemo.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SimCaptcha;
using SimCaptcha.Common;
using SimCaptcha.Models;

namespace ASPNETCoreDemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VCodeController : ControllerBase
    {
        /// <summary>
        /// 验证码配置信息
        /// </summary>
        private readonly SimCaptchaOptions _options;

        private readonly SimCaptchaService _service;

        public VCodeController(IOptions<SimCaptchaOptions> options)
        {
            _options = options.Value;
            this._service = new SimCaptchaService(new LocalCache(), options.Value);
        }

        #region 获取验证码
        /// <summary>
        /// 获取验证码
        /// </summary>
        /// <returns></returns>
        [Route(nameof(VCodeImg))]
        public async Task<IActionResult> VCodeImg()
        {
            // TODO: 产生验证码相关

            return Ok(new { code = 0, message = "" });
        }
        #endregion

        #region 验证码效验
        /// <summary>
        /// 效验验证码
        /// </summary>
        /// <param name="verifyInfo"></param>
        /// <returns></returns>
        [Route(nameof(VCodeCheck))]
        public IActionResult VCodeCheck(VerifyInfoModel verifyInfo)
        {
            VCodeCheckResponseModel responseModel = null;
            // TODO: 获取ip地址
            responseModel = _service.VCodeCheck(verifyInfo, Request.Host.Host);

            return Ok(responseModel);
        }
        #endregion

        #region ticket效验
        public IActionResult TicketVerify(string ticket, string userId, string userIp)
        {
            TicketVerifyResponseModel responseModel = null;
            // TODO: ticket 效验
            responseModel = _service.TicketVerify("", "", ticket, userId, userIp);

            return Ok(responseModel);
        }
        #endregion

    }
}
