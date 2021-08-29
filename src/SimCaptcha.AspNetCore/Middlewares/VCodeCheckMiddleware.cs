using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using SimCaptcha.Models;
using SimCaptcha.Interface;
using Microsoft.Extensions.DependencyInjection;
using SimCaptcha.Models.Click;
using SimCaptcha.Models.Slider;
using SimCaptcha.ResponseModels;
using SimCaptcha.Implement.Click;
using SimCaptcha.Implement.Slider;

namespace SimCaptcha.AspNetCore.Middlewares
{
    /// <summary>
    /// 效验验证码 - 配置好验证码服务端的SimCaptcha.js后, 由SimCaptcha.js自动处理(往返于用户浏览器与验证码服务端，无需业务后台关注)
    /// </summary>
    public class VCodeCheckMiddleware : SimCaptchaMiddleware
    {
        public VCodeCheckMiddleware(RequestDelegate next, IOptionsMonitor<SimCaptchaOptions> optionsAccessor, ICacheHelper cacheHelper, IHttpContextAccessor accessor, IJsonHelper jsonHelper, ILogHelper logHelper) : base(next, optionsAccessor, cacheHelper, accessor, jsonHelper, logHelper)
        { }

        public async Task InvokeAsync(HttpContext context, IServiceProvider serviceProvider)
        {
            string inputBody;
            using (var reader = new System.IO.StreamReader(
                context.Request.Body, Encoding.UTF8))
            {
                inputBody = await reader.ReadToEndAsync();
            }
            VerifyInfoModel verifyInfo = _jsonHelper.Deserialize<VerifyInfoModel>(inputBody);
            VCodeCheckResponseModel responseModel;

            if (!this._cacheHelper.Exists(CachePrefixCaptchaType + verifyInfo.UserId))
            {
                // 验证码无效，1.此验证码已被销毁
                responseModel = new VCodeCheckResponseModel { code = -5, message = "验证码过期, 获取新验证码" };
            }
            else
            {
                string captchaType = this._cacheHelper.Get<string>(CachePrefixCaptchaType + verifyInfo.UserId);
                SimCaptchaService simCaptchaService = null;
                switch (captchaType)
                {
                    case "click":
                        simCaptchaService = serviceProvider.GetService<ClickSimCaptchaService>();
                        verifyInfo = _jsonHelper.Deserialize<ClickVerifyInfoModel>(inputBody);
                        break;
                    case "slider":
                        simCaptchaService = serviceProvider.GetService<SliderSimCaptchaService>();
                        verifyInfo = _jsonHelper.Deserialize<SliderVerifyInfoModel>(inputBody);
                        break;
                    default:
                        simCaptchaService = serviceProvider.GetService<SliderSimCaptchaService>();
                        verifyInfo = _jsonHelper.Deserialize<SliderVerifyInfoModel>(inputBody);
                        break;
                }


                // 获取ip地址
                string userIp = _accessor.HttpContext.Connection.RemoteIpAddress.ToString();
                responseModel = simCaptchaService.VCodeCheck(verifyInfo, userIp);
            }

            string responseJsonStr = _jsonHelper.Serialize(responseModel);

            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(responseJsonStr, Encoding.UTF8);

            // Response.Write 开始, 不要再 Call next
            // Call the next delegate/middleware in the pipeline
            //await _next(context);
        }
    }
}
