using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using SimCaptcha.Interface;
using SimCaptcha.Models;
using SimCaptcha.ResponseModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SimCaptcha.AspNetCore.Middlewares
{
    /// <summary>
    /// ticket效验 - 给业务后台验证使用
    /// </summary>
    public class TicketVerifyMiddleware : SimCaptchaMiddleware
    {
        public TicketVerifyMiddleware(RequestDelegate next, IOptionsMonitor<SimCaptchaOptions> optionsAccessor, ICacheHelper cacheHelper, IHttpContextAccessor accessor, IJsonHelper jsonHelper, ILogHelper logHelper) : base(next, optionsAccessor, cacheHelper, accessor, jsonHelper, logHelper)
        { }

        public async Task InvokeAsync(HttpContext context, SimCaptchaService simCaptchaService)
        {
            string inputBody;
            using (var reader = new System.IO.StreamReader(
                context.Request.Body, Encoding.UTF8))
            {
                inputBody = await reader.ReadToEndAsync();
            }
            TicketVerifyModel ticketVerify = _jsonHelper.Deserialize<TicketVerifyModel>(inputBody);

            // ticket 效验
            TicketVerifyResponseModel responseModel = simCaptchaService.TicketVerify(ticketVerify.AppId, ticketVerify.AppSecret, ticketVerify.Ticket, ticketVerify.UserId, ticketVerify.UserIp);
            string responseJsonStr = _jsonHelper.Serialize(responseModel);

            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(responseJsonStr, Encoding.UTF8);

            // Response.Write 开始, 不要再 Call next
            // Call the next delegate/middleware in the pipeline
            //await _next(context);
        }
    }
}
