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
using SimCaptcha.AspNetCore.Extensions;

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
            // 1.��Ҫ: ע����֤������, ֮��Ϳ����ڿ����� ͨ��������ע��
            services.Configure<SimCaptchaOptions>(Configuration.GetSection(
                SimCaptchaOptions.SimCaptcha));

            #region �������� (��ҵ���̨����֤������ͬԴ, ����Ҫ����)
            SimCaptchaOptions simCaptchaOptions = new SimCaptchaOptions();
            Configuration.GetSection(SimCaptchaOptions.SimCaptcha).Bind(simCaptchaOptions);
            IEnumerable<List<string>> temp = simCaptchaOptions.AppList?.Select(m => m.CorsWhiteList);
            // ������������ Origin
            List<string> allAllowedCorsOrigins = new List<string>();
            foreach (var corsWhiteList in temp)
            {
                foreach (var item in corsWhiteList)
                {
                    allAllowedCorsOrigins.Add(item);
                }
            }

            // ���� AspNetCoreClient ��������
            services.AddCors(options =>
            {
                options.AddPolicy(name: VCodeAllowSpecificOrigins,
                    builder =>
                    {
                        // SimCaptchaOptions �����õİ�����������
                        builder.WithOrigins(allAllowedCorsOrigins.ToArray())

                            // �������json,������������: https://blog.csdn.net/yangyiboshigou/article/details/78738228
                            // �������: Access-Control-Allow-Headers: Content-Type
                            // �ο�: https://www.cnblogs.com/jpfss/p/10102132.html
                            .WithHeaders("Content-Type");
                    });
            }); 
            #endregion

            // 2.��� SimCaptcha
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

            // app.UseHttpsRedirection();

            app.UseRouting();

            // ����: ���� CORS �м�� (��ҵ���̨����֤������ͬԴ, ����Ҫ����)
            // ע��: ��������, ��ʹ����·�� ��ʹ�ô˿������
            app.UseCors(VCodeAllowSpecificOrigins);

            // 3.���� SimCaptcha �м��
            app.UseSimCaptcha();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
