using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SimCaptcha;

namespace AspNetCoreService
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
            // 重要: 注册验证码配置, 之后就可以在控制器 通过构造器注入
            services.Configure<SimCaptchaOptions>(Configuration.GetSection(
                                        SimCaptchaOptions.SimCaptcha));
            // 允许 AspNetCoreClient 跨域请求
            services.AddCors(options =>
            {
                options.AddPolicy(name: VCodeAllowSpecificOrigins,
                                  builder =>
                                  {
                                      builder.WithOrigins("http://example.com",
                                                          "https://localhost:44379");
                                  });
            });

            // 用于获取ip地址
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            // 用于 SimCaptcha.AspNetCore.LocalCache 缓存
            services.AddMemoryCache();

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

            // 跨域: 启用 CORS 中间件
            app.UseCors();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
