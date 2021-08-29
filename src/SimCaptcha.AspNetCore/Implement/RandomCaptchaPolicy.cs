using Microsoft.AspNetCore.Http;
using SimCaptcha.AspNetCore.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimCaptcha.AspNetCore.Implement
{
    public class RandomCaptchaPolicy : ICaptchaPolicy
    {
        public CaptchaType Policy(IHttpContextAccessor httpContextAccessor, IServiceProvider serviceProvider)
        {
            int num = new Random().Next(0, 1);
            if (num == 0)
            {
                return CaptchaType.Click;
                //return CaptchaType.Slider;
            }
            else
            {
                //return CaptchaType.Slider;
                return CaptchaType.Click;
            }
        }
    }
}
