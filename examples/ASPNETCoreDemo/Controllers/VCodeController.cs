using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
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

        public VCodeController(IOptions<SimCaptchaOptions> options)
        {
            _options = options.Value;
        }

        #region 获取验证码
        /// <summary>
        /// 获取验证码
        /// </summary>
        /// <returns></returns>
        [Route(nameof(VCodeImg))]
        public async Task<IActionResult> VCodeImg()
        {


            return Ok(new { code = 0, message = "" });
        }
        #endregion

        #region 效验验证码
        /// <summary>
        /// 效验验证码
        /// </summary>
        /// <param name="verifyInfo"></param>
        /// <returns></returns>
        [Route(nameof(VCodeCheck))]
        public IActionResult VCodeCheck(VerifyInfoModel verifyInfo)
        {
            VCodeCheckResponseModel responseModel = new VCodeCheckResponseModel();

            // AES解密
            string vCodeKeyJsonStr = verifyInfo.VCodeKey;
            // json -> 对象
            VCodeKeyModel vCodeKeyModel = null;
            try
            {
                // 能够转换为 对象，则说明秘钥无误，可以使用
                vCodeKeyModel = JsonSerializer.Deserialize<VCodeKeyModel>(vCodeKeyJsonStr);
            }
            catch (Exception ex)
            { }
            if (vCodeKeyModel == null)
            {
                // 秘钥无效，被篡改
                responseModel.code = -3;
                responseModel.message = "验证码无效, 获取新验证码";
                return Ok(responseModel);
            }
            IList<PointPosModel> rightVCodePos = vCodeKeyModel.VCodePos;
            IList<PointPosModel> userVCodePos = verifyInfo.VCodePos;
            // 验证码是否正确
            bool isPass = false;
            // TODO: 效验点触位置数据

            // 错误次数是否达上限
            bool isMoreThanErrorNum = vCodeKeyModel.ErrorNum > _options.ErrorNum;

            // 验证码是否过期
            bool isExpired = ((DateTimeHelper.NowTimeStamp13() - vCodeKeyModel.TS) / 1000) > _options.ExpiredSec;

            if (!isPass && !isMoreThanErrorNum)
            {
                // 错误 -> 1.code:-1 验证码错误 且 错误次数未达上限 -> message: 点错啦，请重试
                vCodeKeyModel.ErrorNum++;
                string vCodekeyJsonStrTemp = JsonSerializer.Serialize(vCodeKeyModel);
                // AES加密 vCodekeyJsonStrTemp
                string vCodeKeyStrTemp = AesHelper.EncryptEcbMode(vCodekeyJsonStrTemp, _options.AesKey);

                responseModel.code = -1;
                responseModel.message = "点错啦，请重试";
                responseModel.data = new VCodeCheckResponseModel.DataModel { vCodeKey = vCodeKeyStrTemp };
                return Ok(responseModel);
            }
            else if (!isPass && isMoreThanErrorNum)
            {
                // 错误 -> 2.code:-2 验证码错误 且 错误次数已达上限 -> message: 这题有点难，为你换一个试试吧
                responseModel.code = -2;
                responseModel.message = "这题有点难, 为你换一个试试吧";
                return Ok(responseModel);
            }
            else if (isExpired)
            {
                // 验证码过期
                responseModel.code = -4;
                responseModel.message = "验证码过期, 获取新验证码";
                return Ok(responseModel);
            }

            // 正确 -> code:0 下发票据 ticket
            // TODO: ip地址获取
            TicketModel ticketModel = new TicketModel { IP = "", IsPass = true, TS = DateTimeHelper.NowTimeStamp10() };
            string ticketJsonStr = JsonSerializer.Serialize(ticketModel);
            // 对 ticketJsonStr 加密
            string ticketStr = AesHelper.EncryptEcbMode(ticketJsonStr, _options.AesKey);

            responseModel.code = 0;
            responseModel.message = "验证通过";
            responseModel.data = new VCodeCheckResponseModel.DataModel { appId = "", ticket = ticketStr };
            return Ok(responseModel);
        }
        #endregion

    }
}
