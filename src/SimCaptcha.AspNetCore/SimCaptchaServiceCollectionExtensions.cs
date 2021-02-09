using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SimCaptcha.Interface;

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
            services.AddSingleton<IVCodeImage, AspNetCoreVCodeImage>();
            services.AddSingleton<ILogHelper, ConsoleLogHelper>();
            services.AddSingleton<ICache, LocalCache>();
            services.AddSingleton<LocalCache>();

            return services;
        }
    }
}
