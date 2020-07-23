using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SimCaptcha;
using SimCaptcha.AspNetCore;

namespace EasyAspNetCoreService
{
    public class Startup
    {
        readonly string VCodeAllowSpecificOrigins = "_VCodeAllowSpecificOrigins";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // 1.重要: 注册验证码配置, 之后就可以在控制器 通过构造器注入
            services.Configure<SimCaptchaOptions>(Configuration.GetSection(
                SimCaptchaOptions.SimCaptcha));

            #region 跨域配置 (若业务后台与验证码服务端同源, 则不需要配置)
            SimCaptchaOptions simCaptchaOptions = new SimCaptchaOptions();
            Configuration.GetSection(SimCaptchaOptions.SimCaptcha).Bind(simCaptchaOptions);
            IEnumerable<List<string>> temp = simCaptchaOptions.AppList?.Select(m => m.CorsWhiteList);
            // 所有允许跨域的 Origin
            List<string> allAllowedCorsOrigins = new List<string>();
            foreach (var corsWhiteList in temp)
            {
                foreach (var item in corsWhiteList)
                {
                    allAllowedCorsOrigins.Add(item);
                }
            }

            // 允许 AspNetCoreClient 跨域请求
            services.AddCors(options =>
            {
                options.AddPolicy(name: VCodeAllowSpecificOrigins,
                    builder =>
                    {
                        // SimCaptchaOptions 里配置的白名单都允许
                        builder.WithOrigins(allAllowedCorsOrigins.ToArray())

                            // 解决发送json,复杂请求问题: https://blog.csdn.net/yangyiboshigou/article/details/78738228
                            // 解决方法: Access-Control-Allow-Headers: Content-Type
                            // 参考: https://www.cnblogs.com/jpfss/p/10102132.html
                            .WithHeaders("Content-Type");
                    });
            }); 
            #endregion

            // 2.添加 SimCaptcha
            services.AddSimCaptcha();

            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            // 跨域: 启用 CORS 中间件 (若业务后台与验证码服务端同源, 则不需要配置)
            // 注意: 这样启用, 会使所有路由 都使用此跨域策略
            app.UseCors(VCodeAllowSpecificOrigins);

            // 3.启用 SimCaptcha 中间件
            app.UseSimCaptcha();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
