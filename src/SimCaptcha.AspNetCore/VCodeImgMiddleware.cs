using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using SimCaptcha.Models;
using SimCaptcha.Interface;

namespace SimCaptcha.AspNetCore
{
    /// <summary>
    /// 获取验证码 - 配置好验证码服务端的SimCaptcha.js后, 由SimCaptcha.js自动处理(无需业务后台关注)
    /// </summary>
    public class VCodeImgMiddleware : SimCaptchaMiddleware
    {
        public VCodeImgMiddleware(RequestDelegate next, IOptions<SimCaptchaOptions> optionsAccessor, ICache cache, IHttpContextAccessor accessor, IVCodeImage vCodeImage, IJsonHelper jsonHelper, ILogHelper logHelper) : base(next, optionsAccessor, cache, accessor, vCodeImage, jsonHelper, logHelper)
        { }

        public override async Task InvokeAsync(HttpContext context)
        {
            VCodeResponseModel responseModel = await _service.VCode();
            string responseJsonStr = _jsonHelper.Serialize(responseModel);

            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(responseJsonStr, Encoding.UTF8);

            // Response.Write 开始, 不要再 Call next
            // Call the next delegate/middleware in the pipeline
            //await _next(context);
        }
    }
}
