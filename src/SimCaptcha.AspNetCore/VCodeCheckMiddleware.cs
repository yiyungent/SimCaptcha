using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using SimCaptcha.Models;

namespace SimCaptcha.AspNetCore
{
    /// <summary>
    /// 效验验证码 - 配置好验证码服务端的SimCaptcha.js后, 由SimCaptcha.js自动处理(往返于用户浏览器与验证码服务端，无需业务后台关注)
    /// </summary>
    public class VCodeCheckMiddleware : SimCaptchaMiddleware
    {
        public VCodeCheckMiddleware(RequestDelegate next, IOptions<SimCaptchaOptions> optionsAccessor, IMemoryCache memoryCache, IHttpContextAccessor accessor) : base(next, optionsAccessor, memoryCache, accessor)
        { }

        public override async Task InvokeAsync(HttpContext context)
        {
            string inputBody;
            using (var reader = new System.IO.StreamReader(
                context.Request.Body, Encoding.UTF8))
            {
                inputBody = await reader.ReadToEndAsync();
            }
            VerifyInfoModel verifyInfo = _jsonHelper.Deserialize<VerifyInfoModel>(inputBody);

            // 获取ip地址
            string userIp = _accessor.HttpContext.Connection.RemoteIpAddress.ToString();
            VCodeCheckResponseModel responseModel = _service.VCodeCheck(verifyInfo, userIp);
            string responseJsonStr = _jsonHelper.Serialize(responseModel);

            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(responseJsonStr, Encoding.UTF8);

            // Response.Write 开始, 不要再 Call next
            // Call the next delegate/middleware in the pipeline
            //await _next(context);
        }
    }
}
