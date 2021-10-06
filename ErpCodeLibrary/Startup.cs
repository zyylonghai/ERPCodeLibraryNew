using ErpCode.BaseApiController;
using ErpCode.Com;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
namespace ErpCodeLibrary
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            IConfigurationManager.Configuration = configuration;
    
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDistributedRedisCache(option => option.Configuration = "127.0.0.1:6379,abortConnect=false,connectRetry=3,connectTimeout=3000,defaultDatabase=1,syncTimeout=3000,version=3.2.1,responseTimeout=3000");
            //services.AddRazorPages();
            services.AddControllers().AddJsonOptions(options =>
            {
                //��ʽ������ʱ���ʽ
                //options.JsonSerializerOptions.Converters.Add(new DatetimeJsonConverter());
                //���ݸ�ʽ����ĸСд
                //options.JsonSerializerOptions.PropertyNamingPolicy =JsonNamingPolicy.CamelCase;
                //���ݸ�ʽԭ�����
                options.JsonSerializerOptions.PropertyNamingPolicy = null;
                //ȡ��Unicode����
                //options.JsonSerializerOptions.Encoder = JavaScriptEncoder.Create(UnicodeRanges.All);
                //���Կ�ֵ
                //options.JsonSerializerOptions.IgnoreNullValues = true;
                //����������
                //options.JsonSerializerOptions.AllowTrailingCommas = true;
                //�����л����������������Ƿ�ʹ�ò����ִ�Сд�ıȽ�
                //options.JsonSerializerOptions.PropertyNameCaseInsensitive = false;
            });
            services.AddSession();
            //services 
            //services.AddDistributedMemoryCache();
            //services.AddScoped<IRedisClient, CustomerRedis>();
            var csredis = new CSRedis.CSRedisClient("127.0.0.1:6379,abortConnect=false,connectRetry=3,connectTimeout=3000,defaultDatabase=0,syncTimeout=3000,version=3.2.1,responseTimeout=3000");
            RedisHelper.Initialization(csredis);//��ʼ��
            services.AddAuthorization();
            services.AddAuthentication("Bearer")
                .AddJwtBearer(options =>
                {
                    options.Authority = Configuration["IdentityServer:url1"];    //����Identityserver����Ȩ��ַ
                    options.RequireHttpsMetadata = false;           //����Ҫhttps    
                    options.Audience = OAuthConfig.UserApi.Scope;  //api��name����Ҫ��config��������ͬ
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            }
            app.UseMiddleware<LibException>();
            //app.UseHttpsRedirection();
            //app.UseStaticFiles();
            app.UseSession();
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                //endpoints.MapRazorPages();
                endpoints.MapControllers();
            });

            DefaultFilesOptions defaultFilesOptions = new DefaultFilesOptions();
            defaultFilesOptions.DefaultFileNames.Clear();
            defaultFilesOptions.DefaultFileNames.Add("index.html");
            app.UseDefaultFiles(defaultFilesOptions);
            app.UseStaticFiles();
        }
    }
}
