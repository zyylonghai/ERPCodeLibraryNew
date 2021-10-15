using ErpCode.Com;
using ErpRptModels.Com;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ErpCodeLibrary.ApiControllers.COM
{
    [Route("COM/[controller]/[action]/{prog}")]
    [ApiController]
    [Authorize]
    public class CheckrptController:COMController
    {
        protected override void GetReportDataExt()
        {
            base.GetReportDataExt();
            var heard = this.ClientDatas.FirstOrDefault(i => i.TableNm == "CheckRptHeard").ClientDataInfos[0].Datas as CheckRptHeard;
            heard.Title = "20200906001";
            heard.checker = "zhengyiyong";
            heard.checkdt = DateTime .Now .ToString ("yyyy-MM-dd");
            var data = this.ClientDatas.FirstOrDefault(i => i.TableNm == "CheckRptData");
            for (int i = 0; i < 20; i++)
            {
                CheckRptData d = new CheckRptData();
                d.data1 = "kdjsf"+i.ToString ();
                d.data2 = "kkkk";
                d.data3 = "llllsdk";
                d.data4 = "少了几分";
                d.data5 = "skd";
                d.data6 = "老司机";
                data.ClientDataInfos.Add(new LibClientDataInfo { Datas = d });
            }
            //data.Test1 = "FQC202009050001";
            //data.Test2 = "zyylonghai";

        }
    }
}
