using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using SimCaptcha.AspNetCore.Implement;
using SimCaptcha.AspNetCore.Implement.Slider;
using SimCaptcha.AspNetCore.Interface;
using SimCaptcha.Click;
using SimCaptcha.Implement;
using SimCaptcha.Interface;
using SimCaptcha.Interface.Slider;
using SimCaptcha.Slider;

namespace SimCaptcha.AspNetCore
{
    public static class SimCaptchaServiceCollectionExtensions
    {
        public static IServiceCollection AddSimCaptcha(
            this IServiceCollection services)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            // 1.用于 SimCaptcha.AspNetCore.LocalCache 缓存
            services.AddMemoryCache();
            // 2.用于获取ip地址
            //services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.TryAdd(ServiceDescriptor.Singleton<IHttpContextAccessor, HttpContextAccessor>());
            services.AddSingleton<IJsonHelper, AspNetCoreJsonHelper>();
            services.AddSingleton<ILogHelper, ConsoleLogHelper>();
            services.AddSingleton<ICacheHelper, CacheHelper>();
            services.AddSingleton<ICache>((serviceProvider) =>
            {
                SimCaptchaOptions options = serviceProvider.GetService<IOptionsMonitor<SimCaptchaOptions>>().CurrentValue;
                IMemoryCache memoryCache = serviceProvider.GetService<IMemoryCache>();
                ICache cache = new LocalCache(memoryCache);
                cache.TimeOut = options.ExpiredSec;

                return cache;
            });
            services.AddSingleton<LocalCache>();
            services.AddSingleton<IEncryptHelper, AesEncryptHelper>();
            services.AddSingleton<IAppChecker, DefaultAppChecker>();
            services.AddTransient<ISimCaptchaOptions>((serviceProvider) =>
            {
                SimCaptchaOptions options = serviceProvider.GetService<IOptionsMonitor<SimCaptchaOptions>>().CurrentValue;

                return options;
            });
            services.AddTransient<SimCaptchaOptions>((serviceProvider) =>
            {
                SimCaptchaOptions options = serviceProvider.GetService<IOptionsMonitor<SimCaptchaOptions>>().CurrentValue;

                return options;
            });
            services.AddTransient<ClickSimCaptchaService>();
            services.AddTransient<SliderSimCaptchaService>();

            services.AddSingleton<ICaptchaPolicy, RandomCaptchaPolicy>();

            services.AddSingleton<IClickVCodeImage, ClickVCodeImage>();
            services.AddSingleton<IClickRandomCode, ClickRandomCodeHanZi>();
            services.AddSingleton<ISliderVCodeImage, SliderVCodeImage>();


            services.AddTransient<SimCaptchaService>((serviceProvider) =>
            {
                ICaptchaPolicy captchaPolicy = serviceProvider.GetService<ICaptchaPolicy>();
                IHttpContextAccessor httpContextAccessor = serviceProvider.GetService<IHttpContextAccessor>();
                var capatchaType = captchaPolicy.Policy(httpContextAccessor, serviceProvider);
                SimCaptchaService simCaptchaService = null;
                switch (capatchaType)
                {
                    case CaptchaType.Click:
                        simCaptchaService = serviceProvider.GetService<ClickSimCaptchaService>();
                        break;
                    case CaptchaType.Slider:
                        simCaptchaService = serviceProvider.GetService<SliderSimCaptchaService>();
                        break;
                    default:
                        simCaptchaService = serviceProvider.GetService<SliderSimCaptchaService>();
                        break;
                }

                return simCaptchaService;
            });

            return services;
        }
    }
}
