using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimCaptcha.AspNetCore.Interface
{
    public interface ICaptchaPolicy
    {
        CaptchaType Policy(IHttpContextAccessor httpContextAccessor, IServiceProvider serviceProvider);
    }
}
