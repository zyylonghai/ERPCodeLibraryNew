using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ErpCode.Identity.Server.Validator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ErpCode.Identity.Server
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            #region 数据库存储方式
            services.AddIdentityServer()
                .AddDeveloperSigningCredential()
                .AddInMemoryApiResources(OAuthMemoryData.GetApiResources())
                //.AddInMemoryClients(OAuthMemoryData.GetClients())
                .AddClientStore<ClientStore>()
                //.AddResourceOwnerValidator<ResourceOwnerPasswordValidator>()
                .AddResourceOwnerValidator<ResourceOwnerPasswordValidator>();
                //.AddTestUsers(OAuthMemoryData.GetTestUsers());
                //.AddExtensionGrantValidator<WeiXinOpenGrantValidator>()
                //.AddProfileService<UserProfileService>();//添加微信端自定义方式的验证
            #endregion
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();
            app.UseIdentityServer();
            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
