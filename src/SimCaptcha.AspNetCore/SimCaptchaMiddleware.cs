using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using SimCaptcha.Interface;
using Microsoft.Extensions.Caching.Memory;

namespace SimCaptcha.AspNetCore
{
    public abstract class SimCaptchaMiddleware
    {
        protected readonly RequestDelegate _next;

        protected readonly IOptionsMonitor<SimCaptchaOptions> _optionsAccessor;

        protected readonly IJsonHelper _jsonHelper;

        protected readonly IHttpContextAccessor _accessor;

        protected readonly ILogHelper _logHelper;

        public SimCaptchaMiddleware(RequestDelegate next, IOptionsMonitor<SimCaptchaOptions> optionsAccessor, ICache cache, IHttpContextAccessor accessor, IJsonHelper jsonHelper, ILogHelper logHelper)
        {
            _next = next;
            _optionsAccessor = optionsAccessor;

            // 注意: 这意外着 更新 ExpiredSec 必须重启站点 才能生效
            cache.TimeOut = optionsAccessor.CurrentValue.ExpiredSec;

            _accessor = accessor;
            _jsonHelper = jsonHelper;
            _logHelper = logHelper;
        }

        public abstract Task InvokeAsync(HttpContext context, SimCaptchaService simCaptchaService);
    }
}
