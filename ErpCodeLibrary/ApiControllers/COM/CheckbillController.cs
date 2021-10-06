using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ErpCode.Com;
using ErpModels.Com;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ErpCodeLibrary.ApiControllers.COM
{
    [Route("COM/[controller]/[action]/{prog}")]
    [ApiController]
    [Authorize]
    public class CheckbillController : COMController
    {
        protected override void PageLoadExt()
        {
            if (this.OperatAction == LibOperatAction.Add)
            {
                var checkbill = this.ClientDatas.FirstOrDefault(i => i.TableNm == "CheckBill");
                if (checkbill != null)
                {
                    CheckBill checkBill = checkbill.ClientDataInfos[0].Datas as CheckBill;
                    //checkBill.BillNo = "202008060001";
                    checkBill.CheckDT = DateTime.Now;
                    checkBill.CreateDT = DateTime.Now;
                    checkBill.remark = "lsjflsdjfkds";
                    checkBill.CompanyId = "zyy";
                    checkBill.departmentId = "IT";
                }
                //LibClientDatas checkmaps = this.ClientDatas.FirstOrDefault(i => i.TableNm == "CheckBillMap");
                //for (int i = 0; i < 2; i++)
                //{
                //    LibClientDataInfo dataInfo = new LibClientDataInfo();
                //    dataInfo.clientDataStatus = LibClientDataStatus.Add;
                //    dataInfo.Datas = new CheckBillMap { BillNo = "202008060001", RowNo = i, check1 = "zyy" };
                //    checkmaps.ClientDataInfos.Add(dataInfo);
                //}
            }
            //this.ThrowErrorException("zyytestyichang");
            //this.AddMessage("错误测试");
            //var checkBillMap = this.ClientDatas.FirstOrDefault(i => i.TableNm == "CheckBillMap");
            //if (checkBillMap != null)
            //{
            //    CheckBillMap billMap = new CheckBillMap();
            //    billMap.BillNo = "202007170001";
            //    billMap.check1 = "zyytest";
            //    LibClientDataInfo dataInfo = new LibClientDataInfo();
            //    dataInfo.Datas = billMap;
            //    dataInfo.clientDataStatus = LibClientDataStatus.Add;
            //    checkBillMap.ClientDataInfos.Add(dataInfo);
            //}
            base.PageLoadExt();
            //this.ThrowErrorException("异常日志测试");
        }

        protected override void AppEventProcessExt(LibEventHandle ehandle)
        {
            var checkbill = this.ClientDatas.FirstOrDefault(i => i.TableNm == "CheckBill");
            if (checkbill != null)
            {
                CheckBill checkBill = checkbill.ClientDataInfos[0].Datas as CheckBill;
                checkBill.BillNo = ehandle.EventSource.FieldValue;
            }
            var checkBillMap = this.ClientDatas.FirstOrDefault(i => i.TableNm == "CheckBillMap");
            if (checkBillMap != null)
            {
                CheckBillMap billMap = new CheckBillMap();
                billMap.BillNo = ehandle .EventSource .FieldValue;
                billMap.check1 = "zyytest";
                LibClientDataInfo dataInfo = new LibClientDataInfo();
                dataInfo.Datas = billMap;
                dataInfo.clientDataStatus = LibClientDataStatus.Add;
                checkBillMap.ClientDataInfos.Add(dataInfo);
            }
            base.AppEventProcessExt(ehandle);
        }

        protected override void TableActionExt(LibClientDatas clientDatas)
        {
            base.TableActionExt(clientDatas);
            var checkmap = ClientDataToModel<CheckBillMap>(clientDatas.ClientDataInfos);
            if (clientDatas.ClientDataInfos[0].clientDataStatus == LibClientDataStatus.Add)
                checkmap[0].check2 = "333test";
        }
    }
}