using ErpCode.AppDal;
using ErpCode.BaseApiController;
using ErpCode.Com;
using ErpCode.Com.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ViewModels.AppViewModel;

namespace ErpCodeLibrary.ApiControllers.AppSys
{
    [Route("AppSys/[controller]/[action]/{prog}")]
    [ApiController]
    //[Authorize]
    public class ProgController : DataBaseController<ProgDal>
    {
        public string GetProgData(int limit, int page)
        {
            int total = this.tDal.dBContext.ProgInfo.Count();
            var data = this.tDal.dBContext.ProgInfo.ToList().Skip(limit * (page - 1)).Take(limit).ToList();
            var result = new { code = 0, msg = "success", count = total, data = data };

            return JsonConvert.SerializeObject(result);
        }

        //public string GetProgDataById(string prognm, int limit, int page)
        //{
        //    var data = this.tDal.dBContext.ProgInfo.FirstOrDefault(i => i.ProgNm == prognm);
        //    if(data)

        //}

        
        public ResponseMsg GetProgInfoData(string prognm)
        {
            this.ProgNm = System .Web .HttpUtility .UrlDecode(prognm);
            ProgModels data = this.SessionData.ProgInfoData;
            if (data == null)
            {
                data = this.tDal.GetProgInfoData(this.ProgNm);
                if (data != null)
                {
                    #region  权限验证
                    var authobjs = this.tDal.CheckAuthorityObjs();
                    var auths = authobjs.AuthorityObjs.Where(i => i != null && i.ProgNm.ToUpper() == prognm).ToList();
                    if (authobjs.Joles.FirstOrDefault(i => i.IsAdminJole) == null)
                    {
                        if (authobjs.JoleDs == null || authobjs.JoleDs.FirstOrDefault(i => i != null && i.ProgNm.ToUpper() == prognm) == null)
                        {
                            //msg000000007 功能{0},无使用权限
                            this.ThrowErrorException(7, prognm);
                        }
                    }
                    foreach (var item in auths)
                    {
                        if (item == null) continue;
                        if (item.AuthorityType == LibAuthorityType.Control)
                        {
                            var exist = data.progControlInfos.FirstOrDefault(i => i.ID == item.ControlID);
                            if (exist != null && item.IsHide)
                                data.progControlInfos.Remove(exist);
                        }
                        else if (item.AuthorityType == LibAuthorityType.Field)
                        {
                            var exis = data.progFieldInfos.FirstOrDefault(i => i.ID == item.ControlID && i.Field == item.Field);
                            if (exis != null)
                            {
                                exis.IsHide = item.IsHide;
                                exis.OnlyRead = item.OnlyRead;
                            }
                        }
                    }
                    #endregion
                    this.SessionData.ProgInfoData = data;
                }
                //this.SessionData.AddDataExt(new SessionDataExtTest { A = "kdsjfksl", Models = new ProgModels() });
            }
            if (data == null)
            {
                //msg000000004  未找到功能{0}
                this.ThrowErrorMessage();
                //this.ThrowErrorException(4, this.ProgNm);
            }
            //this.ProgNm = data.progInfo.progNm;
            //var proginfo = progdatas.First().a;
            //var progcontrols = progdatas.GroupBy(i => i.b).Select (i=>i.Key).ToList ();
            //var progfields = progdatas.GroupBy(i => i.c).Select(i => i.Key).ToList();
            //List<LibClientDatas> clientdatas = new List<LibClientDatas>();
            LibClientDatas prog = new LibClientDatas();
            LibClientDataInfo dataInfo;
            prog.TableNm = "ProgInfo";
            dataInfo = new LibClientDataInfo();
            switch (this.OperatAction)
            {
                case LibOperatAction.Add:
                default:
                    dataInfo.clientDataStatus = LibClientDataStatus.Add;
                    break;
                case LibOperatAction.Edit:
                    dataInfo.clientDataStatus = LibClientDataStatus.Edit;
                    break;
                case LibOperatAction.Preview:
                    dataInfo.clientDataStatus = LibClientDataStatus.Preview;
                    break;
                //default:
                //    dataInfo.clientDataStatus = LibClientDataStatus.Add;
                //    break;
            }
            //dataInfo.clientDataStatus = LibClientDataStatus.Preview;
            dataInfo.Datas =data.progInfo;
            prog.ClientDataInfos = new List<LibClientDataInfo>();
            prog.ClientDataInfos.Add(dataInfo);
            //clientdatas.Add(libClient);

            LibClientDatas controlinfos = new LibClientDatas();
            controlinfos.TableNm = "ProgControlInfo";
            dataInfo = new LibClientDataInfo();
            dataInfo.clientDataStatus = LibClientDataStatus.Preview;
            dataInfo.Datas = data.progControlInfos;
            controlinfos.ClientDataInfos = new List<LibClientDataInfo>();
            controlinfos.ClientDataInfos.Add(dataInfo);
            //foreach (var item in data.progControlInfos)
            //{
            //    this.SessionData.TableNms.Add(item.TableNm);
            //}
            //clientdatas.Add(libClient);

            LibClientDatas fields = new LibClientDatas();
            fields.TableNm = "ProgFieldInfo";
            dataInfo = new LibClientDataInfo();
            dataInfo.clientDataStatus = LibClientDataStatus.Preview;
            dataInfo.Datas = data.progFieldInfos;
            fields.ClientDataInfos = new List<LibClientDataInfo>();
            fields.ClientDataInfos.Add(dataInfo);
            //clientdatas.Add(libClient);
            //throw new Exception("异常测试");
            return new ResponseMsg {IsSuccess =true , Data =new {proginfo=prog , controlinfos = controlinfos,fieldinfos=fields }, Message = "取数成功" };
        }

        //public ResponseMsg GetProgInfoData2(string pgnm)
        //{
        //    ProgModels data = null;
        //    this.ProgNm = pgnm;
        //    //ProgModels data = this.SessionData.ProgInfoData;
        //    //if (data == null)
        //    //{
        //        data = this.tDal.GetProgInfoData(pgnm);
        //    this.SessionData.ProgInfoData = data;
        //    //}
        //    //if (data == null)
        //    //{
        //    //    //msg000000004  未找到功能{0}
        //    //    this.ThrowErrorException(4, this.ProgNm);
        //    //}
        //    LibClientDatas prog = new LibClientDatas();
        //    LibClientDataInfo dataInfo;
        //    prog.TableNm = "ProgInfo";
        //    dataInfo = new LibClientDataInfo();

        //    dataInfo.Datas = data.progInfo;
        //    prog.ClientDataInfos = new List<LibClientDataInfo>();
        //    prog.ClientDataInfos.Add(dataInfo);


        //    LibClientDatas controlinfos = new LibClientDatas();
        //    controlinfos.TableNm = "ProgControlInfo";
        //    dataInfo = new LibClientDataInfo();
        //    dataInfo.clientDataStatus = LibClientDataStatus.Preview;
        //    dataInfo.Datas = data.progControlInfos;
        //    controlinfos.ClientDataInfos = new List<LibClientDataInfo>();
        //    controlinfos.ClientDataInfos.Add(dataInfo);

        //    LibClientDatas fields = new LibClientDatas();
        //    fields.TableNm = "ProgFieldInfo";
        //    dataInfo = new LibClientDataInfo();
        //    dataInfo.clientDataStatus = LibClientDataStatus.Preview;
        //    dataInfo.Datas = data.progFieldInfos;
        //    fields.ClientDataInfos = new List<LibClientDataInfo>();
        //    fields.ClientDataInfos.Add(dataInfo);
        //    return new ResponseMsg { IsSuccess = true, Data = new { proginfo = prog, controlinfos = controlinfos, fieldinfos = fields }, Message = "取数成功" };
        //}

        /// <summary>
        /// 主要用于查询功能后，选中某行进行编辑
        /// </summary>
        /// <param name="prognm"></param>
        /// <returns></returns>
        public ResponseMsg GetProgInfoDataNoAuthority(string prognm)
        {
            this.ProgNm = System.Web.HttpUtility.UrlDecode(prognm);
            ProgModels data = this.SessionData.ProgInfoData;
            if (data == null)
            {
                data = this.tDal.GetProgInfoData(this.ProgNm);
                this.SessionData.ProgInfoData = data;
                this.SessionData.AddDataExt(new SessionDataExtTest { A = "kdsjfksl", Models = new ProgModels() });
            }
            if (data == null)
            {
                //msg000000004  未找到功能{0}
                this.ThrowErrorException(4, this.ProgNm);
            }
            LibClientDatas prog = new LibClientDatas();
            LibClientDataInfo dataInfo;
            prog.TableNm = "ProgInfo";
            dataInfo = new LibClientDataInfo();
            switch (this.OperatAction)
            {
                case LibOperatAction.Add:
                default:
                    dataInfo.clientDataStatus = LibClientDataStatus.Add;
                    break;
                case LibOperatAction.Edit:
                    dataInfo.clientDataStatus = LibClientDataStatus.Edit;
                    break;
                case LibOperatAction.Preview:
                    dataInfo.clientDataStatus = LibClientDataStatus.Preview;
                    break;
            }

            dataInfo.Datas = data.progInfo;
            prog.ClientDataInfos = new List<LibClientDataInfo>();
            prog.ClientDataInfos.Add(dataInfo);


            LibClientDatas controlinfos = new LibClientDatas();
            controlinfos.TableNm = "ProgControlInfo";
            dataInfo = new LibClientDataInfo();
            dataInfo.clientDataStatus = LibClientDataStatus.Preview;
            dataInfo.Datas = data.progControlInfos;
            controlinfos.ClientDataInfos = new List<LibClientDataInfo>();
            controlinfos.ClientDataInfos.Add(dataInfo);

            LibClientDatas fields = new LibClientDatas();
            fields.TableNm = "ProgFieldInfo";
            dataInfo = new LibClientDataInfo();
            dataInfo.clientDataStatus = LibClientDataStatus.Preview;
            dataInfo.Datas = data.progFieldInfos;
            fields.ClientDataInfos = new List<LibClientDataInfo>();
            fields.ClientDataInfos.Add(dataInfo);
            return new ResponseMsg { IsSuccess = true, Data = new { proginfo = prog, controlinfos = controlinfos, fieldinfos = fields }, Message = "取数成功" };
        }

        public ResponseMsg GetRptInfoData(string prognm)
        {
            this.ProgNm = System.Web.HttpUtility.UrlDecode(prognm);
            ProgModels data = this.SessionData.ProgInfoData;
            if (data == null)
            {
                data = this.tDal.GetRptProgInfoData(this.ProgNm);
                this.SessionData.ProgInfoData = data;
            }
            if (data == null)
            {
                //msg000000004  未找到功能{0}
                this.ThrowErrorException(4, this.ProgNm);
            }
            LibClientDatas prog = new LibClientDatas();
            LibClientDataInfo dataInfo;
            prog.TableNm = "ProgInfo";
            dataInfo = new LibClientDataInfo();
            dataInfo.Datas = data.progInfo;
            prog.ClientDataInfos = new List<LibClientDataInfo>();
            prog.ClientDataInfos.Add(dataInfo);

            LibClientDatas controlinfos = new LibClientDatas();
            controlinfos.TableNm = "ProgControlInfo";
            dataInfo = new LibClientDataInfo();
            dataInfo.clientDataStatus = LibClientDataStatus.Preview;
            dataInfo.Datas = data.progControlInfos;
            controlinfos.ClientDataInfos = new List<LibClientDataInfo>();
            controlinfos.ClientDataInfos.Add(dataInfo);

            LibClientDatas fields = new LibClientDatas();
            fields.TableNm = "ProgFieldInfo";
            dataInfo = new LibClientDataInfo();
            dataInfo.clientDataStatus = LibClientDataStatus.Preview;
            dataInfo.Datas = data.progFieldInfos;
            fields.ClientDataInfos = new List<LibClientDataInfo>();
            fields.ClientDataInfos.Add(dataInfo);
            return new ResponseMsg { IsSuccess = true, Data = new { proginfo = prog,htmlstr=data.RptHtmlInfo.HtmlStr , controlinfos = controlinfos, fieldinfos = fields }, Message = "取数成功" };

        }

        protected override void BeforeUpdate()
        {
            base.BeforeUpdate();
            //var ctrls = this.ClientDatas.FirstOrDefault(i => i.TableNm == "ProgControlInfo");
            //var fields = this.ClientDatas.FirstOrDefault(i => i.TableNm == "ProgFieldInfo");
            //if (fields != null)
            //{
            //   var ctrls2=  ClientDataToModel<ProgControlInfo>(ctrls.ClientDataInfos);
            //    //ctrls .ClientDataInfos =(List<ProgControlInfo>)ctrls .ClientDataInfos 
            //    if (fields.ClientDataInfos != null && fields.ClientDataInfos.Count > 0)
            //    {
            //        foreach (LibClientDataInfo info in fields.ClientDataInfos)
            //        {
            //            if (info.Datas.GetType().Equals(typeof(JObject)))
            //            {
            //                ProgFieldInfo fieldInfo = JsonConvert.DeserializeObject<ProgFieldInfo>(info.Datas.ToString());
            //                if (ctrls2.FirstOrDefault(i => i.ID == fieldInfo.ID && i.ControlType == LibControlType.Grid)!=null) continue;
            //                var jobj = ((JObject)info.Datas).Children();
            //                foreach (JProperty p in jobj)
            //                {
            //                    if (p.Name.ToLower() == "field")
            //                    {
            //                        p.Value = string.Format("{0}{1}", p.Value.ToString().Substring(0, 1).ToLower(), p.Value.ToString().Substring(1));
            //                    }
            //                }
            //            }
            //            //f.Field = string.Format("{0}{1}", f.Field.Substring(0, 1).ToLower(), f.Field.Substring(1));
            //        }
            //    }
            //}
        }
    }

    public class SessionDataExtTest
    {
        public string A { get; set; }
        public ProgModels Models { get; set; }
    }
}