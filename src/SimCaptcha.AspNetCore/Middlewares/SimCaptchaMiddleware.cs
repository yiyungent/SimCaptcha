using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using SimCaptcha.Interface;
using Microsoft.Extensions.Caching.Memory;

namespace SimCaptcha.AspNetCore.Middlewares
{
    public abstract class SimCaptchaMiddleware
    {
        protected readonly RequestDelegate _next;

        protected readonly IOptionsMonitor<SimCaptchaOptions> _optionsAccessor;

        protected readonly IJsonHelper _jsonHelper;

        protected readonly IHttpContextAccessor _accessor;

        protected readonly ILogHelper _logHelper;

        protected readonly ICacheHelper _cacheHelper;

        protected const string CachePrefixCaptchaType = "Cache:SimCaptcha:CaptchaType:";

        public SimCaptchaMiddleware(RequestDelegate next, IOptionsMonitor<SimCaptchaOptions> optionsAccessor, ICacheHelper cacheHelper, IHttpContextAccessor accessor, IJsonHelper jsonHelper, ILogHelper logHelper)
        {
            _next = next;
            _optionsAccessor = optionsAccessor;
            _accessor = accessor;
            _jsonHelper = jsonHelper;
            _logHelper = logHelper;
            _cacheHelper = cacheHelper;
        }
    }
}
