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

        protected readonly SimCaptchaOptions _options;

        protected readonly SimCaptchaService _service;

        protected readonly IJsonHelper _jsonHelper;

        protected readonly IHttpContextAccessor _accessor;

        public SimCaptchaMiddleware(RequestDelegate next, IOptions<SimCaptchaOptions> optionsAccessor, ICache cache, IHttpContextAccessor accessor, IVCodeImage vCodeImage, IJsonHelper jsonHelper, ILogHelper logHelper)
        {
            _next = next;
            _options = optionsAccessor.Value;

            cache.TimeOut = optionsAccessor.Value.ExpiredSec;

            _service = new SimCaptchaService(
                optionsAccessor.Value,
                cache,
                vCodeImage,
                jsonHelper,
                logHelper
                );
            _accessor = accessor;
            _jsonHelper = jsonHelper;
        }

        public abstract Task InvokeAsync(HttpContext context);
    }
}
