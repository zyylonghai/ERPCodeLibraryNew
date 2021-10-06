using Library.Core;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ErpCode.BaseApiController
{
    public class LibException
    {
        private readonly RequestDelegate next;
        private IHostingEnvironment environment;
        LogHelp logHelp = null;

        public LibException(RequestDelegate next, IHostingEnvironment environment)
        {
            this.next = next;
            this.environment = environment;
            logHelp = new LogHelp(environment.WebRootPath + "//");
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await next.Invoke(context);
                var features = context.Features;
            }
            catch (Exception e)
            {
                await HandleException(context, e);
            }
        }

        private async Task HandleException(HttpContext context, Exception e)
        {
            context.Response.StatusCode = 500;
            context.Response.ContentType = "text/json;charset=utf-8;";
            string error = "";
            //string msg = "";
            //if (environment.IsDevelopment())
            //{
                while (e.InnerException != null)
                {
                    e = e.InnerException;
                }
                var json = new { message = e.Message };
                error = JsonConvert.SerializeObject(json);
            //}
            //else
            //{
               
            //    error = JsonConvert.SerializeObject(new { message = "抱歉，出错了" });
            //}
            #region 写日志
            logHelp.WriteLog(context.Request.Path, e);
            #endregion 
            await context.Response.WriteAsync(error);
        }
    }
}
