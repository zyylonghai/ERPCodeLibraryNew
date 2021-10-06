using ErpCode.Com;
using Library.Core;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Runtime.Caching;
using System.Text;

namespace ErpCode.BaseApiController
{
    public class LibClientDataChangeMonitor : LibChangeMonitorBase
    {
        private string _prognm = string.Empty;
        private List<LibClientDatas> _clientdatas = null;
        private HttpContext _httpContext;
        public LibClientDataChangeMonitor(string uniqueid, List<LibClientDatas> clientdata, string prognm, HttpContext httpContext)
        {
            this._uniqueid = uniqueid;
            this._prognm = prognm;
            this._clientdatas = clientdata;
            this._httpContext = httpContext;
            this.InitializationComplete();
        }
        public override string UniqueId { get { return this._uniqueid; } }
        public override void CacheRemoveCallBack(CacheEntryUpdateArguments arg)
        {
            RedisHelper.Set(this._uniqueid, this._clientdatas, 1200);
            //this._httpContext.Session .SetObject(this._uniqueid, this._clientdatas);
        }
    }
}
