﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using SimCaptcha.AspNetCore.Middlewares;
using SimCaptcha.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace SimCaptcha.AspNetCore.Extensions
{
    public static class SimCaptchaMiddlewareExtensions
    {
        public static IApplicationBuilder UseSimCaptcha(this IApplicationBuilder builder)
        {
            builder.Map("/api/SimCaptcha/Img", app => app.UseMiddleware<VCodeImgMiddleware>());
            builder.Map("/api/SimCaptcha/Check", app => app.UseMiddleware<VCodeCheckMiddleware>());
            builder.Map("/api/SimCaptcha/TicketVerify", app => app.UseMiddleware<TicketVerifyMiddleware>());

            return builder;
        }

        public static IApplicationBuilder UseSimCaptcha(this IApplicationBuilder builder, SimCaptchaOptions options)
        {
            builder.Map("/api/SimCaptcha/Img", app => app.UseMiddleware<VCodeImgMiddleware>(new OptionsWrapper<SimCaptchaOptions>(options)));
            builder.Map("/api/SimCaptcha/Check", app => app.UseMiddleware<VCodeCheckMiddleware>(new OptionsWrapper<SimCaptchaOptions>(options)));
            builder.Map("/api/SimCaptcha/TicketVerify", app => app.UseMiddleware<TicketVerifyMiddleware>(new OptionsWrapper<SimCaptchaOptions>(options)));

            return builder;
        }
    }
}
