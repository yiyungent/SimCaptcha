using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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
                                      // TODO: 测试,在Asp.Net Core中, Action实体形参只能传json, 不能用application/x-www-form-urlencoded, 否则 HTTP 415, 原因分析: 应该是因为客户端发送数据时用的json格式，没有转换为FormData格式
                                      // 参考: https://www.cnblogs.com/jpfss/p/10102132.html
                                      .WithHeaders("Content-Type");
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
