using ErpCode.AppDal;
using ErpCode.BaseApiController;
using ErpModels.Appsys;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ErpCodeLibrary.ApiControllers.AppSys
{
    [Route("AppSys/[controller]/[action]/{prog}")]
    [ApiController]
    //[Authorize]
    public class MultiLanguageController :DataBaseController<AppsysDal>
    {
        public string GetLanguageData(string progNm,string tbnm,string fieldnm,string val, int limit, int page)
        {
            int total = 0;
            var data= this.tDal.GetLanguageData(progNm, tbnm, fieldnm, val, limit, page, out total);
            return ReturnGridData("", total, data);
        }
    }
}
