using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ErpCode.AuthorDal;
using ErpCode.BaseApiController;
using ErpCode.Com;
using ErpCode.Com.Enums;
using ErpModels;
using ErpModels.Author;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ErpCodeLibrary.ApiControllers.Author
{
    [Route("Author/[controller]/[action]/{prog}")]
    [ApiController]
    [Authorize]
    public class JoleController : DataBaseController<JoleDal>
    {
        protected override void PageLoadExt()
        {
            base.PageLoadExt();
            //if (this.OperatAction == LibOperatAction.Edit ||this.OperatAction==LibOperatAction.Preview)
            //{
            var exit = this.ClientDatas.FirstOrDefault(i => i.TableNm == "AuthorityObj");
            if (exit == null)
            {
                var jole = this.ClientDatas.FirstOrDefault(i => i.TableNm == "Jole").ClientDataInfos[0].Datas as Jole;
                List<AuthorityObj> data = this.tDal.GetAuthorityObjs(jole.JoleId);
                LibClientDatas clientDatas = new LibClientDatas();
                clientDatas.TableNm = "AuthorityObj";
                foreach (var item in data)
                {
                    LibClientDataInfo dataInfo = new LibClientDataInfo();
                    dataInfo.Datas = item;
                    clientDatas.ClientDataInfos.Add(dataInfo);
                }
                this.ClientDatas.Add(clientDatas);
            }
                //clientDatas.cli
            //}
        }

        protected override void BeforeUpdate()
        {
            base.BeforeUpdate();

        }
        protected override void TableActionExt(LibClientDatas clientDatas)
        {
            base.TableActionExt(clientDatas);
            var exit = this.ClientDatas.FirstOrDefault(i => i.DataSource == clientDatas.DataSource && i.TableNm == clientDatas.TableNm);
            if (exit != null && exit.ClientDataInfos != null)
            {
                var dt = clientDatas.ClientDataInfos[0].Datas as JoleD;
                //if (clientDatas.ClientDataInfos[0].clientDataStatus == LibClientDataStatus.Add)
                //{
                    foreach (var item in exit.ClientDataInfos)
                    {
                        var o = item.Datas as JoleD;
                        if (o.ProgNm == dt.ProgNm) {
                            this.ThrowErrorException("重复的prognm");
                        }

                    }
                //}
            }
        }

        public string GetControlAuthorityData(string pgnm)
        {
            var data = this.tDal.GetProgControls(pgnm);
            var data2 =new  List<object>();
            var old = this.ClientDatas.FirstOrDefault(i => i.TableNm == "AuthorityObj");
            var olddata = ClientDataToModel<AuthorityObj>(old.ClientDataInfos);
            if (olddata == null) olddata = new List<AuthorityObj>();
            if (data != null)
            {
                foreach (var item in data)
                {
                    var exis = olddata.FirstOrDefault(i => i.AuthorityType == LibAuthorityType.Control && i.ControlID == item.ID);
                    data2.Add(new { ControlID = item.ID, ControlType = item.ControlType, Title = item.Title,LAY_CHECKED=exis==null?false:exis.IsHide});
                }
            }
            return ReturnGridData("success", data.Count, data2);
        }

        public string GetfieldAuthorityData(string pgnm)
        {
            var old = this.ClientDatas.FirstOrDefault(i => i.TableNm == "AuthorityObj");
            var olddata = ClientDataToModel<AuthorityObj>(old.ClientDataInfos);
            var data = this.tDal.GetProgFields(pgnm, olddata);
            //foreach (var item in olddata)
            //{
                
            //}
            return ReturnGridData("",data.Count (), data);
        }

        public ResponseMsg SaveAuthorityobj(List<AuthorityObj> clientDatas)
        {
            var old = this.ClientDatas.FirstOrDefault(i => i.TableNm == "AuthorityObj");
            var olddata = ClientDataToModel<AuthorityObj>(old.ClientDataInfos);
            if (olddata == null) olddata = new List<AuthorityObj>();
            olddata.Where(i => i.AuthorityType == LibAuthorityType.Control).ToList().ForEach(i => { 
                i.IsHide = false; 
                i.LibModelStatus = LibModelStatus.Edit; 
            });
            foreach (var item in clientDatas)
            {
                var exis =olddata.FirstOrDefault(i => i.ControlID == item.ControlID && i.Field == item.Field);
                if (exis == null)
                {
                    LibClientDataInfo dataInfo = new LibClientDataInfo();
                    dataInfo.clientDataStatus = LibClientDataStatus.Add;
                    dataInfo.Datas = item;
                    item.LibModelStatus = LibModelStatus.Add;
                    item.SeqNo =olddata.Count ==0?1: olddata.Max(i => i.SeqNo) + 1;
                    olddata.Add(item);
                    old.ClientDataInfos.Add(dataInfo);
                    continue;
                }
                exis.IsHide = item.IsHide;
                exis.OnlyRead = item.OnlyRead;
                exis.LibModelStatus = LibModelStatus.Edit;
            }
            return ResponseMsg;
        }
    }
}