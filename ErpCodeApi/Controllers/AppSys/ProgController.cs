using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BaseApiController;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace ErpCodeApi.Controllers.AppSys
{
    [Route("AppSys/[controller]/[action]")]
    [ApiController]
    public class ProgController : AppSysController
    {
        public string GetProgData()
        {
            var data = this.dBContext.ProgInfo.ToList();
            var result = new { code = 0, msg = "success", count = data.Count, data = data };

            return JsonConvert.SerializeObject(result);
        }

        public ResponseMsg GetProgInfoData(string prognm)
        {
            var proginfo = this.dBContext.ProgInfo.FirstOrDefault(i => i.ProgNm == prognm);
            throw new Exception("异常测试");
            return new ResponseMsg { Data = "", Message = "" };
        }
    }
}