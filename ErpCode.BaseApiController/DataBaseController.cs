using ErpCode.Com;
using LibDBContext;
using Library.Core;
using LibraryBaseDal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using System.Text;
using ErpModels.Appsys;
using Newtonsoft.Json.Linq;
using System.Runtime.Caching;
using ErpModels.Com;
using ErpModels;
using System.Reflection;
using ErpCode.Com.Enums;
using Library.Core.LibAttribute;
using System.ComponentModel.DataAnnotations;
using ErpModels.AppDataLog;

namespace ErpCode.BaseApiController
{
    [ApiController]
    public class DataBaseController<TDal> : ControllerBase, IActionFilter
        where TDal: LibBaseDal
    {
        #region 私有属性
        private TDal _tdal=null;
        private LibSessionData _sessiondata=null;
        private LibClientUserInfo _userinfo = null;
        //private LibLanguage _language;
        private ResponseMsg _responsemsg = null;
        private CachHelp _cachHelp = null;
        private DateTimeOffset TimeOffset { get { return DateTime.Now.AddSeconds(30); } }
        private DateTimeOffset languageTimeOffset { get { return DateTime.Now.AddSeconds(1200); } }
        private string _key { get { return string.Format("{0}_{1}", HttpContext.Session.Id, this.ProgNm); } }
        #endregion
        #region 公开属性
        public string ProgNm { get; set; }
        public TDal tDal { get { return _tdal; } }
        public List<LibClientDatas> ClientDatas { get; set; }
        public LibClientUserInfo UserInfo {
            get {
                if (_userinfo == null)
                {
                    _userinfo = HttpContext.Session.GetObject<LibClientUserInfo>(AppConstManage.appuserinfo);
                }
                return _userinfo;
            }
            set
            {
                this._userinfo = value;
            }
        }
        public LibOperatAction OperatAction { get { return this.SessionData == null ? this.OperatAction : this.SessionData.OperatAction; } }
        public ResponseMsg ResponseMsg {
            get
            {
                if (_responsemsg == null)
                {
                    _responsemsg = new ResponseMsg();
                    _responsemsg.IsSuccess = true;
                    _responsemsg.showMessageBox = false;
                    _responsemsg.Message = "";
                }
                if (this._tdal.Messages != null && this._tdal.Messages.Count > 0)
                {
                    var error = this._tdal.Messages.Where(i => i.MsgType == LibMessageType.Error).ToList();
                    _responsemsg.IsSuccess = !(error != null && error.Count > 0);
                    if (!_responsemsg.IsSuccess)
                    {
                        _responsemsg.showMessageBox = true;
                        //_responsemsg.Message = "error";
                        _responsemsg.MessageType = 1;
                        foreach (var item in error)
                        {
                            if (!string.IsNullOrEmpty(_responsemsg.Message))
                            {
                                _responsemsg.Message += "<br/>";
                            }
                            _responsemsg.Message += item.Message;
                        }
                    }
                    else
                    {
                        _responsemsg.showMessageBox = this.tDal.Messages.Count > 0;
                        foreach (var item in this.tDal.Messages)
                        {
                            if (!string.IsNullOrEmpty(_responsemsg.Message))
                            {
                                _responsemsg.Message += "<br/>";
                            }
                            _responsemsg.Message += item.Message;
                        }
                    }
                }
                if (_responsemsg.IsSuccess)
                {
                    if (this.SessionData.ProgInfoData != null)
                    {
                        foreach (var item in this.SessionData.ProgInfoData.progControlInfos)
                        {
                            if (item.ControlType == LibControlType.Colla || item.ControlType ==LibControlType.RptTable || item.ControlType ==LibControlType.RptGrid)
                            {
                                if (_responsemsg.ClientDatas == null) _responsemsg.ClientDatas = new List<LibClientDatas>();
                                if (_responsemsg.ClientDatas.FirstOrDefault(i => i.DataSource == item.DataSourceNm && i.TableNm == item.TableNm) == null)
                                {
                                    var o = this.ClientDatas.FirstOrDefault(i => i.DataSource == item.DataSourceNm && i.TableNm == item.TableNm);
                                    if (o != null)
                                        _responsemsg.ClientDatas.Add(o);
                                }
                            }

                        }
                    }
                }
                return _responsemsg;
            } 
        }
        public LibSessionData SessionData
        {
            get
            {
                if (string.IsNullOrEmpty(this.ProgNm)) return _sessiondata;

                if (_sessiondata == null)
                {
                    _sessiondata = HttpContext.Session.GetObject<LibSessionData>(this.ProgNm);
                    if (_sessiondata == null) _sessiondata = new LibSessionData();
                }
                return _sessiondata;
            }
        }

        public LibLanguage LibLanguage { 
            get {
                if (this.UserInfo != null)
                    return (LibLanguage)this.UserInfo.Language;
                else
                    return 0;
            }
        }
        #endregion
        #region 构造函数
        public DataBaseController()
        {
            _cachHelp = new CachHelp();
            _tdal = Activator.CreateInstance<TDal>();
        }
        #endregion

        #region 公开函数
        /// <summary>页面加载</summary>
        /// <returns></returns>
        public virtual ResponseMsg PageLoaded()
        {
            Type t;
            LibClientDatas clientDatas = null;
            LibClientDataInfo dataInfo = null;
            //if (this.IsDeveloper() || this.IsDeveloperProg())
            //{
                if (this.OperatAction == LibOperatAction.Add || this.OperatAction == LibOperatAction.None)
                {
                    this.ClientDatas.Clear();
                    if (this.SessionData.ProgInfoData != null)
                    {
                        foreach (var item in this.SessionData.ProgInfoData.progControlInfos)
                        {
                            if (item == null) continue;
                            clientDatas = new LibClientDatas();
                            clientDatas.TableNm = item.TableNm;
                            clientDatas.DataSource = item.DataSourceNm;
                            if (item.ControlType == LibControlType.Colla)
                            {
                                clientDatas.collas.Add(item.ID);
                                dataInfo = new LibClientDataInfo();
                                dataInfo.clientDataStatus = LibClientDataStatus.Add;
                                t = tDal.GetTypeBytModelNm(item.TableNm);
                                dataInfo.Datas = Activator.CreateInstance(t);
                                clientDatas.ClientDataInfos.Add(dataInfo);
                                #region 解析默认值表达式
                                var fs = this.SessionData.ProgInfoData.progFieldInfos.Where(i => i.ID == item.ID && !i.IsHide && !string.IsNullOrEmpty(i.DefaultValue.Trim()));
                                if (fs != null)
                                {
                                    PropertyInfo[] ps = t.GetProperties();
                                    PropertyInfo o = null;
                                    foreach (var f in fs)
                                    {
                                        string[] expesslist = f.DefaultValue.Split(';');
                                        if (expesslist != null)
                                        {
                                            o = ps.FirstOrDefault(i => i.Name == f.Field);
                                            if (o == null) continue;
                                            foreach (string exp in expesslist)
                                            {
                                                if (exp.StartsWith("ConstVal", StringComparison.OrdinalIgnoreCase))//赋常量
                                                {
                                                    //if (o != null)
                                                    //{
                                                    string v = LibAppUtils.GetBracketContent(exp, BracketType.Parentheses);
                                                    v = v.Replace("\"", "");
                                                    if (o.PropertyType.Equals(typeof(decimal)) || o.PropertyType.Equals(typeof(int)) ||
                                                        o.PropertyType.Equals(typeof(double)) || o.PropertyType.Equals(typeof(long)))
                                                    {
                                                        o.SetValue(dataInfo.Datas, Convert.ToDecimal(v));
                                                    }
                                                    else
                                                        o.SetValue(dataInfo.Datas, v);
                                                    //}
                                                }
                                                else if (exp.StartsWith("GetCurrentDate", StringComparison.OrdinalIgnoreCase))//取当前日期
                                                {
                                                    if (o.PropertyType.Equals(typeof(DateTime)))
                                                    {
                                                        o.SetValue(dataInfo.Datas, Convert.ToDateTime(System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")));
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                #endregion
                            }
                            var exit = this.ClientDatas.FirstOrDefault(i => i.DataSource == item.DataSourceNm && i.TableNm == item.TableNm);
                            if (exit == null)
                            {
                                clientDatas.CollectIdentity(this.tDal.GetTypeBytModelNm(clientDatas.TableNm));
                                this.ClientDatas.Add(clientDatas);

                            }
                            else
                            {
                                exit.collas.Add(item.ID);
                            }
                        }
                    }
                }
                else if (this.OperatAction == LibOperatAction.Edit)
                {

                }
                PageLoadExt();
            //}
            //else//租户自定义开发
            //{
            //    UserDevPageLoad();
            //}
            var rsp = ResponseMsg;
            rsp.Data = this.OperatAction;
            //rsp.Message = "has PageLoaded";
            return rsp;
        }

        /// <summary>事件处理</summary>
        /// <param name="ehandle"></param>
        /// <returns></returns>
        [HttpPost]
        public  ResponseMsg AppEventProcess(LibEventHandle ehandle)
        {
            //byte[] str = new byte[10000];
            //HttpContext.Request.Body.ReadAsync(str, 0, str.Length);
            AppEventProcessExt(ehandle);
            var response = ResponseMsg;
            //response.showMessageBox = false;
            return response;
        }

        /// <summary>保存数据</summary>
        /// <param name="clientdata"></param>
        /// <returns></returns>
        [HttpPost]
        public virtual ResponseMsg Update(List<LibClientDatas> clientdata)
        {
            //ResponseMsg response = new ResponseMsg();
            if (string.Compare (this.ProgNm,"ProgInfo",true)==0)
            {
                this.ClientDatas.Clear();
                foreach (LibClientDatas item in clientdata)
                {
                    foreach (LibClientDataInfo info in item.ClientDataInfos)
                    {
                        info.Datas = JsonConvert.DeserializeObject(((System.Text.Json.JsonElement)info.Datas).ToString());
                    }
                    this.ClientDatas.Add(item);
                }
            }
            else
            {
                foreach (LibClientDatas item in clientdata)
                {
                    var data = this.ClientDatas.FirstOrDefault(i => i.DataSource == item.DataSource && i.TableNm == item.TableNm);
                    //if (data!=null && (data.TableNm == "ProgControlInfo" || data.TableNm == "ProgFieldInfo")) continue;
                    foreach (LibClientDataInfo info in item.ClientDataInfos)
                    {
                        info.Datas = JsonConvert.DeserializeObject(((System.Text.Json.JsonElement)info.Datas).ToString());
                        if (data != null && data.ClientDataInfos != null)
                        {
                            foreach (LibClientDataInfo d in data.ClientDataInfos)
                            {
                                LibAppUtils.MapToFieldValue(info, d);

                                d.clientDataStatus = d.clientDataStatus == LibClientDataStatus.Preview ? LibClientDataStatus.Edit : d.clientDataStatus;
                            }
                        }

                    }
                    if (data == null)
                        this.ClientDatas.Add(item);
                }
                #region 将主表中的主键字段值，赋值到明细表中的与其同名的主键字段
                Dictionary<string, object> keys = new Dictionary<string, object>();
                //有面板元素，即必须要配置主表
                var hascolas = this.SessionData.ProgInfoData.progControlInfos.FirstOrDefault(i => i.ControlType == LibControlType.Colla)!=null;
                if (string.IsNullOrEmpty(this.SessionData.ProgInfoData.progInfo.mastTable) && hascolas )
                {
                    //msg000000005 功能{0},未设置主表
                    this.ThrowErrorException(5, this.ProgNm);
                }
                var mtb = this.ClientDatas.FirstOrDefault(i => i.TableNm == this.SessionData.ProgInfoData.progInfo.mastTable);
                if (mtb != null)
                {
                    Type t = this.tDal.GetTypeBytModelNm(mtb.TableNm);
                    PropertyInfo[] ps = t.GetProperties();
                    foreach (PropertyInfo p in ps)
                    {
                        if (p.GetCustomAttribute<KeyAttribute>() != null)
                        {
                            if (keys.ContainsKey(p.Name)) continue;
                            var keyval = p.GetValue(mtb.ClientDataInfos[0].Datas);
                            #region 主表主键值的编码规则处理 没有面板元素，无须执行该代码
                            if (string.IsNullOrEmpty(keyval.ToString()) && hascolas)
                            {
                                p.SetValue(mtb.ClientDataInfos[0].Datas, this.tDal.GenerateNoByprogNm(this.ProgNm));
                            }
                            #endregion 
                            keys.Add(p.Name, p.GetValue(mtb.ClientDataInfos[0].Datas));
                        }
                    }
                }
                Type t2;
                PropertyInfo[] ps2 = null;
                foreach (var item in this.ClientDatas)
                {
                    if (item.DataSource == mtb.DataSource && item.TableNm == mtb.TableNm) continue;
                    t2 = this.tDal.GetTypeBytModelNm(item.TableNm);
                    if (t2 == null) continue;
                    ps2 = t2.GetProperties();
                    foreach (var d in item.ClientDataInfos)
                    {
                        foreach (KeyValuePair<string, object> keyValue in keys)
                        {
                            var p = ps2.FirstOrDefault(i => i.Name == keyValue.Key);
                            if (p == null) continue;
                            p.SetValue(d.Datas, keyValue.Value);
                        }
                    }
                }
                #endregion
            }
            #region 字段值有效验证

            #endregion 
            //this.ClientDatas = clientdata;
            BeforeUpdate();
            if (this._tdal.Messages != null && this._tdal.Messages.Count > 0)
            {
                var error = this._tdal.Messages.Where(i => i.MsgType == LibMessageType.Error).ToList();
                return ResponseMsg;
            }
            this._tdal.Update(this.ClientDatas);
            AfterUpdate();
            //msg000000001 保存成功
            this.AddMessage(1, LibMessageType.Prompt);
            this.SessionData.OperatAction = LibOperatAction.Preview;
            this.AcceptClientData();
            return ResponseMsg;
        }

        /// <summary>
        /// 新增
        /// </summary>
        /// <returns></returns>
        public ResponseMsg Add()
        {
            if (this.ClientDatas != null) this.ClientDatas.Clear();
            this.SessionData.OperatAction = LibOperatAction.Add;
            return ResponseMsg;
        }

        public ResponseMsg Edit()
        {
            if (this.OperatAction == LibOperatAction.Preview)
            {
                this.SessionData.OperatAction = LibOperatAction.Edit;

                foreach (var item in this.ClientDatas)
                {
                    foreach (var d in item.ClientDataInfos)
                    {
                        d.clientDataStatus = LibClientDataStatus.Edit;
                    }
                }
            }
            return ResponseMsg;
        }

        //public ResponseMsg Refresh()
        //{
        //    if (this.ClientDatas != null) this.ClientDatas.Clear();
        //    this.SessionData.OperatAction = LibOperatAction.None;
        //    return ResponseMsg;
        //}
        #region 搜索 相关
        ///// <summary>数据查询</summary>
        ///// <returns></returns>
        //public virtual ResponseMsg SearchData()
        //{
        //    return ResponseMsg;
        //}

        /// <summary></summary>
        /// <param name="tbnm"></param>
        /// <param name="flag">1 标识功能搜索，2 标识来源主数据搜索</param>
        /// <returns></returns>
        public virtual ResponseMsg GetSearchfieldandsymbol(string tbnm,int flag)
        {
            List<LibFieldInfo> libFields = new List<LibFieldInfo>();
            Type t = null;
            string mtb = tbnm;
            Dictionary<string, List<LibEnumkeyvalue>> optionfields = new Dictionary<string, List<LibEnumkeyvalue>>();
            if (flag == 2)
            {
                //参数tbnm不为空，则获取该表的字段。
                if (!string.IsNullOrEmpty(mtb))
                {
                    t = this.tDal.GetTypeBytModelNm(mtb);
                    if (t == null)
                    {
                        this.ThrowErrorException(string.Format("表{0},未取到对应实体类型。", mtb));
                    }
                }
                else
                    this.ThrowErrorException("参数tbnm 不能为空");
            }
            else
            {
                //否则获取功能主表的字段
                if (this.SessionData.ProgInfoData != null)
                {
                    mtb = this.SessionData.ProgInfoData.progInfo.mastTable;
                    t = this.tDal.GetTypeBytModelNm(mtb);
                    if (t == null)
                    {
                        //msg000000005 功能{0},未设置主表
                        this.ThrowErrorException(5, this.ProgNm);
                    }

                }
            }
            if (t != null)
            {
                PropertyInfo[] ps = t.GetProperties();
                if (ps != null && ps.Length > 0)
                {
                    LibFieldInfo fieldInfo = null;
                    foreach (PropertyInfo p in ps)
                    {
                        if (p.Name == AppConstManage.applogid || p.Name == "LibModelStatus" || p.Name == "LAY_CHECKED"||p.Name == "ClientId") continue;
                        fieldInfo = new LibFieldInfo { fieldNm = p.Name, fieldDesc = AppGetFieldDesc(mtb, p.Name) };
                        if (p.PropertyType.BaseType.Equals(typeof(Enum)))
                        {
                            fieldInfo.fieldType = LibFieldType.Enums;
                            optionfields.Add(p.Name, LibAppUtils.GetenumFields(p.PropertyType));
                        }
                        libFields.Add(fieldInfo);
                    }
                }
            }
            SetSearchFieldsExt(tbnm, (LibSearchKind)flag,ref libFields);
            List<LibEnumkeyvalue> symbols = LibAppUtils.GetenumFields<SmodalSymbol>();
            symbols.ForEach(i => { i.value = AppGetFieldDesc(string.Empty, "SmodalSymbol", i.value); });
            List<LibEnumkeyvalue> logic = LibAppUtils.GetenumFields<Smodallogic>();
            logic.ForEach(i => { i.value = AppGetFieldDesc(string.Empty, "", i.value); });
            ResponseMsg response = ResponseMsg;
            response.Data = new { fields = libFields, symbol = symbols,logic=logic,optionfields=optionfields };
            return response;
        }

        public ResponseMsg GetSearchSymbol()
        {
            List<LibEnumkeyvalue> symbols = LibAppUtils.GetenumFields<SmodalSymbol>();
            ResponseMsg response = ResponseMsg;
            response.Data = symbols;
            return response;
        }

        [HttpPost]
        public ResponseMsg SearchData(List<LibSearchCondition> conditions)
        {
            //object[] values = { };
            //StringBuilder whereformat = new StringBuilder();
            //SearchConditionHelper.AnalyzeSearchCondition(conditions, whereformat, ref values);
            //WhereObject where = new WhereObject { WhereFormat = whereformat.ToString (), Values = values };
            this.SessionData.AddDataExt(conditions);
            //CachHelp cach = new CachHelp();
            //cach.AddCachItem(string.Format("{0}_{1}searchcond", HttpContext.Session.Id, this.ProgNm), where, TimeOffset);
            //var data = this.tDal.SearchData(this.SessionData.ProgInfoData.progInfo.mastTable, where);
            return ResponseMsg;
        }

        public virtual string BindSearchDataGrid(string tbnm,LibSearchKind kind, int limit, int page)
        {
            List<LibSearchCondition> conds = this.SessionData.GetDataExt<List<LibSearchCondition>>();
            object[] values = { };
            conds.Add(new LibSearchCondition { FieldNm = "ClientId", Logic = Smodallogic.And, Symbol = SmodalSymbol.Equal, valu1 = this.UserInfo.ClientId });
            StringBuilder whereformat = new StringBuilder();
            SearchConditionHelper.AnalyzeSearchCondition(conds, whereformat, ref values);
            WhereObject where = new WhereObject { WhereFormat = whereformat.ToString(), Values = values };
            //CachHelp cach = new CachHelp();
            //WhereObject where =(WhereObject) cach.GetCach(string.Format("{0}_{1}searchcond", HttpContext.Session.Id, this.ProgNm));
            var data = this.tDal.SearchData(string .IsNullOrEmpty(tbnm)? this.SessionData.ProgInfoData.progInfo.mastTable:tbnm, where);
            SearchDataExt(string.IsNullOrEmpty(tbnm) ? this.SessionData.ProgInfoData.progInfo.mastTable : tbnm, kind,ref data);
            var result = new { code = 0, msg = "success", count = data.Count, data = data };
            return JsonConvert.SerializeObject(result);
        }
        /// <summary> </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public virtual ResponseMsg FillSearchData(object o)
        {
            this.SessionData.OperatAction = LibOperatAction.Preview;
            Type t = this.tDal.GetTypeBytModelNm(this.SessionData.ProgInfoData.progInfo.mastTable);
            object obj= JsonConvert.DeserializeObject(o.ToString());
            var ps = ((JObject)obj).Children();
            PropertyInfo[] keyps = t.GetProperties().Where(i => i.GetCustomAttribute<KeyAttribute>() != null).ToArray ();
            Dictionary<string, object> keys = new Dictionary<string, object>();
            foreach (PropertyInfo p in keyps)
            {
                if (!keys.ContainsKey(p.Name))
                {
                    foreach (JProperty item in ps)
                    {
                        if (item.Name == p.Name)
                        {
                            keys.Add(p.Name, item.Value.ToObject(p.PropertyType));
                        }
                    }
                }
            }
            LibClientDataInfo dataInfo = null;
            foreach (var item in this.ClientDatas)
            {
                if (string.IsNullOrEmpty(item.TableNm)) continue;
                var data = this.tDal.FillSearchData(keys, item.TableNm);
                if (item.ClientDataInfos == null) item.ClientDataInfos = new List<LibClientDataInfo>();
                else item.ClientDataInfos.Clear();
                foreach (var d in data)
                {
                    dataInfo = new LibClientDataInfo();
                    dataInfo.Datas = d;
                    dataInfo.clientDataStatus = LibClientDataStatus.Preview;
                    item.ClientDataInfos.Add(dataInfo);
                }
                item.SetTableIdentityCurrValue();
            }
            return ResponseMsg;
        }
        //public 
        #endregion

        #region 来源主数据搜索相关
        public ResponseMsg GetSearchFromSrcNm(string pgnm, string controlid,string field)
        {
            //List<ProgControlInfo> ctrls = this.SessionData.ProgInfoData.progControlInfos;
            List<ProgControlInfo> ctrls = this.Getsessiondata(pgnm.ToUpper ()).ProgInfoData.progControlInfos;
            if (ctrls != null)
            {
               var exist=ctrls.FirstOrDefault(i => i.ID == controlid);
                if (exist != null)
                {
                    Type t = this.tDal.GetTypeBytModelNm(exist.TableNm);
                    PropertyInfo[] ps = t.GetProperties();
                    var v = ps.FirstOrDefault(i => i.GetCustomAttribute<LibFromSourceAttribute>() != null && i.Name ==field);
                    if (v != null)
                    {
                        var p = v.GetCustomAttribute<LibFromSourceAttribute>();
                        ResponseMsg.Data =new { tbnm = p.FromTableNm ,fromfield=p.FromFieldNm,desc=p.Desc};
                    }
                    else
                    {
                        this.ThrowErrorException(string.Format("表{0},字段{1},未设置来源主数据属性.",exist.TableNm,field));
                    }
                }
            }
            return ResponseMsg;
        }
        #endregion 

        #region 表格相关
        public virtual string  BindDataGrid(string gridid,string ds,string tbnm, int limit,int page)
        {
            var clientdata = this.ClientDatas.FirstOrDefault(i => i.DataSource == ds && i.TableNm == tbnm);
            List<object> data = new List<object> ();
            if (clientdata != null)
            {
                data = clientdata.ClientDataInfos.Where(i=>i.clientDataStatus!=LibClientDataStatus.Delete)
                                                 .Select(i => i.Datas).Skip(limit*(page-1) ).Take (limit).ToList ();
            }
            var result = new { code = 0, msg = "success", count =clientdata ==null?0: clientdata.ClientDataInfos.Count, data = data};

            return JsonConvert.SerializeObject(result);
        }
        [HttpPost]
        public virtual ResponseMsg GetTableAction(LibClientDatas clientDatas)
        {
            ConvertToobj(clientDatas);
            var exist = this.ClientDatas.FirstOrDefault(i => i.DataSource == clientDatas.DataSource && i.TableNm == clientDatas.TableNm);
            if (exist != null)
            {
                foreach (var item in clientDatas.ClientDataInfos)
                {
                    exist.SetIdentityValue(item.Datas);
                }
            }
            GetTableActionExt(clientDatas);
            ResponseMsg response = ResponseMsg;
            response.ClientDatas = new List<LibClientDatas>();
            response.ClientDatas.Add(clientDatas);
            return response;
        }
        [HttpPost]
        public virtual ResponseMsg TableAction(LibClientDatas clientDatas)
        {
            //string applogid = string.Empty;
            //if (clientDatas != null&& clientDatas .ClientDataInfos!=null && clientDatas .ClientDataInfos .Count >0)
            //{
            //    var o = JsonConvert.DeserializeObject<JObject>(clientDatas.ClientDataInfos[0].Datas.ToString());
            //    var ps = o.Children();
            //    foreach (JProperty p in ps)
            //    {
            //        if (p.Name == AppConstManage.applogid)
            //        {
            //            applogid = p.Value.ToString();
            //            break;
            //        }
            //    }
            //}
            ConvertToobj(clientDatas);
            TableActionExt(clientDatas);
            if (this.ClientDatas != null)
            {
                var exit = this.ClientDatas.FirstOrDefault(i => i.DataSource == clientDatas.DataSource && i.TableNm == clientDatas.TableNm);
                if (exit != null)
                {

                    if (exit.ClientDataInfos == null) exit.ClientDataInfos = new List<LibClientDataInfo>();
                    if (exit.ClientDataInfos.Count == 0)
                    {
                        //ConvertToobj(clientDatas);
                        //TableActionExt(clientDatas);
                        exit.ClientDataInfos.AddRange(clientDatas.ClientDataInfos);
                    }
                    else
                    {
                        foreach (var info in clientDatas.ClientDataInfos)
                        {
                            if (info.clientDataStatus == LibClientDataStatus.Add)
                            {
                                //info.Datas = LibAppUtils.JobjectToType(info, this.tDal.GetTypeBytModelNm(clientDatas.TableNm));
                                //TableActionExt(clientDatas);
                                exit.ClientDataInfos.Add(info);
                            }
                            else if (info.clientDataStatus == LibClientDataStatus.Edit)
                            {

                                LibModelCore mcore = info.Datas as LibModelCore;
                                foreach (var v in exit.ClientDataInfos)
                                {
                                    LibModelCore tmcore = v.Datas as LibModelCore;
                                    if (tmcore.app_logid == mcore.app_logid)
                                    {
                                        if (v.clientDataStatus == LibClientDataStatus.Edit)
                                        {
                                            #region 找出主键列的值
                                            if (v.PrimaryKeyInfos == null) v.PrimaryKeyInfos = new List<LibPrimaryKeyInfo>();
                                            v.PrimaryKeyInfos.Clear();
                                            //Dictionary<string, object> keys = new Dictionary<string, object>();
                                            Type tp = this.tDal.GetTypeBytModelNm(exit.TableNm);
                                            PropertyInfo[] ps = tp.GetProperties();
                                            object v1, v2;
                                            bool updatekey = false;
                                            foreach (PropertyInfo p in ps)
                                            {
                                                if (p.GetCustomAttribute<KeyAttribute>() != null)
                                                {
                                                    v1 = p.GetValue(tmcore);
                                                    v2 = p.GetValue(mcore);
                                                    if (!v1.Equals(v2))
                                                    {
                                                        updatekey = true;
                                                    }
                                                    v.PrimaryKeyInfos.Add(new LibPrimaryKeyInfo { KeyColumn = p.Name, Value = p.GetValue(tmcore) });
                                                    //if (keys.ContainsKey(p.Name)) continue;
                                                    //keys.Add(p.Name, p.GetValue(tmcore));
                                                }
                                            }
                                            #endregion
                                            if (updatekey)
                                                v.OldDatas = LibAppUtils.CopyFrom(v.Datas);
                                            else
                                                v.PrimaryKeyInfos = null;
                                        }
                                        LibAppUtils.MapToFieldValue(info, v, true);
                                        //var pkeys = ps.Where(i => keys.Keys.Contains(i.Name));
                                        //foreach (PropertyInfo p in pkeys)
                                        //{
                                        //    p.SetValue(tmcore, keys[p.Name]);
                                        //}
                                        v.clientDataStatus = v.clientDataStatus == LibClientDataStatus.Add ? v.clientDataStatus : info.clientDataStatus;
                                    }
                                }
                                //TableActionExt(exit);
                            }
                            else if (info.clientDataStatus == LibClientDataStatus.Delete)
                            {
                                LibModelCore mcore = info.Datas as LibModelCore;
                                foreach (var v in exit.ClientDataInfos)
                                {
                                    LibModelCore tmcore = v.Datas as LibModelCore;
                                    if (mcore.app_logid == tmcore.app_logid)
                                    {
                                        if (v.clientDataStatus == LibClientDataStatus.Add)
                                        {
                                            exit.ClientDataInfos.Remove(v);
                                        }
                                        else
                                            v.clientDataStatus = LibClientDataStatus.Delete;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    //exit.ClientDataInfos.AddRange(clientDatas.ClientDataInfos);
                }
                //TableActionExt(exit);
            }
            return ResponseMsg;
        }
        #endregion

        #region 数据日志查询
        public ResponseMsg SearchDataLogs()
        {
            List<LibClientDatas> logdatas = new List<LibClientDatas>();
            if (this.ClientDatas != null)
            {
                LibClientDatas clientDatas = null;
                foreach (var item in this.ClientDatas)
                {
                    clientDatas = logdatas.FirstOrDefault(i => i.TableNm == item.TableNm);
                    if (clientDatas == null)
                    {
                        clientDatas = new LibClientDatas();
                        clientDatas.TableNm = item.TableNm;
                        clientDatas.DataSource = item.DataSource;
                        clientDatas.ClientDataInfos = new List<LibClientDataInfo>();
                        logdatas.Add(clientDatas);
                    }
                    //Type t = this.tDal.GetTypeBytModelNm(item.TableNm);
                    //PropertyInfo[] ps = t.GetProperties();
                    foreach (LibClientDataInfo d in item.ClientDataInfos)
                    {
                        List<DataLogsD> logs = this.tDal.SearchDataLogs(item.TableNm, (d.Datas as LibModelCore).app_logid);
                        foreach (DataLogsD l in logs)
                        {
                            clientDatas.ClientDataInfos.Add(new LibClientDataInfo { Datas = l });
                        }
                    }
                }
            }
            return new ResponseMsg {IsSuccess =true ,ClientDatas = logdatas };
        }
        #endregion

        #region 报表相关
        public ResponseMsg GetReportData()
        {
            Type t;
            LibClientDatas clientDatas = null;
            LibClientDataInfo dataInfo = null;
            this.ClientDatas.Clear();
            foreach (var item in this.SessionData.ProgInfoData.progControlInfos)
            {
                if (item == null) continue;
                clientDatas = new LibClientDatas();
                clientDatas.TableNm = item.TableNm;
                clientDatas.DataSource = item.DataSourceNm;
                clientDatas.collas.Add(item.ID);
                dataInfo = new LibClientDataInfo();
                dataInfo.clientDataStatus = LibClientDataStatus.Preview;
                t = tDal.GetTypeBytModelNm(item.TableNm);
                dataInfo.Datas = Activator.CreateInstance(t);
                clientDatas.ClientDataInfos.Add(dataInfo);
                var exit = this.ClientDatas.FirstOrDefault(i => i.DataSource == item.DataSourceNm && i.TableNm == item.TableNm);
                if (exit == null)
                {
                    this.ClientDatas.Add(clientDatas);

                }
                else
                {
                    exit.collas.Add(item.ID);
                }
            }
            GetReportDataExt();
            return ResponseMsg;
        }
        #endregion 

        /// <summary>
        ///如果是页面处于刷新或提交则页面输出的是一串错误信息的json字符，否则以消息弹出框的形式弹出。
        /// </summary>
        /// <param name="msg">信息</param>
        public void ThrowErrorException(string msg)
        {
            ExceptionHelp help = new ExceptionHelp();
            help.ThrowError(this, msg);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="msgcode">信息代码</param>
        /// <param name="parms">formatter 中的参数值</param>
        public void ThrowErrorException(int msgcode, params object[] parms)
        {
            string message =AppGetMessageDesc(msgcode);
            ExceptionHelp help = new ExceptionHelp();
            help.ThrowError(this, string.Format(message, parms));
        }

        /// <summary>
        /// 直接抛出当前tDal中Messages集合 类型为error的信息。
        /// </summary>
        public void ThrowErrorMessage()
        {
            StringBuilder msg = new StringBuilder();
            if (this.tDal.Messages.Count > 0)
            {
                foreach (var item in this.tDal.Messages)
                {
                    if (item.MsgType == LibMessageType.Error)
                        msg.Append(item.Message);
                }
            }
            this.ThrowErrorException(msg.ToString());
        }

        public string AppGetFieldDesc(string prognm, string tbnm, string fieldnm)
        {
            LanguageField v = null;
            CachHelp cachHelp = new CachHelp();
            List<LanguageField> data =(List<LanguageField>)cachHelp.GetCach(prognm);
            if (data == null || data.Count <= 0)
            {
                data = this.tDal.AppGetFieldDesc((int)this.LibLanguage, prognm, string.Empty, string.Empty);
                cachHelp.AddCachItem(prognm, data, languageTimeOffset);
                v = data.FirstOrDefault(i => i.TableNm == tbnm && i.FieldNm == fieldnm);
                if (v != null)
                    return v.Vals;
            }
            v = data.FirstOrDefault(i => i.TableNm == tbnm && i.FieldNm == fieldnm);
            if (v != null )
                return v.Vals;
            return fieldnm;
        }
        public string AppGetFieldDesc(string tbnm, string fieldnm)
        {
            return AppGetFieldDesc(this.ProgNm, tbnm, fieldnm);
        }
        public string AppGetMessageDesc(int msgcode)
        {
            return AppGetFieldDesc(string.Empty, string.Empty, string.Format("msg{0}", msgcode.ToString().PadLeft(9, '0')));
        }

        public LibSessionData Getsessiondata(string prognm)
        {
           return HttpContext.Session.GetObject<LibSessionData>(prognm);
        }

        public void ClearCach()
        {
            _cachHelp.ClearCache();
            RedisHelper.Del(this._key);
        }
        public void ClearSession()
        {
            this._sessiondata = null;
            this._userinfo = null;
        }

        #endregion

        #region 受保护函数
        protected void AddMessage(string msg, LibMessageType type=LibMessageType.Error)
        {
            //this._tdal.Messages.Add(new LibMessage { Message = msg, MsgType = type });
            this._tdal.AddMessage(msg, type);
        }
        protected void AddMessage(int msgcode, LibMessageType type = LibMessageType.Error, params object[] parms)
        {
            this.tDal.AddMessage(string.Format(AppGetMessageDesc(msgcode),parms), type);
        }

        protected List<TModel> ClientDataToModel<TModel>(List<LibClientDataInfo> clientdatas)
        {
            List<TModel> result = null;
            if (clientdatas != null)
            {
                result = new List<TModel>();
                foreach (LibClientDataInfo d in clientdatas)
                {
                    if (d == null) continue;
                    if (d.Datas != null)
                    {
                        Type t = d.Datas.GetType();
                        if (t.Equals(typeof(JObject)))
                            d.Datas = JsonConvert.DeserializeObject<TModel>(d.Datas.ToString());
                        else if (t.Equals(typeof(System.Text.Json.JsonElement)))
                        {
                            d.Datas = JsonConvert.DeserializeObject(((System.Text.Json.JsonElement)d.Datas).ToString());
                            d.Datas = JsonConvert.DeserializeObject<TModel>(d.Datas.ToString());
                        }
                        result.Add((TModel)d.Datas);
                    }
                }
            }
            return result;
        }

        protected void AcceptClientData()
        {
            if (this.ClientDatas != null)
            {
                foreach (var item in this.ClientDatas)
                {
                    foreach (var d in item.ClientDataInfos)
                    {
                        d.clientDataStatus = LibClientDataStatus.Preview;
                    }
                }
            }
        }

        protected string ReturnGridData(string msg, int totalRow, object data)
        {
            var result = new { code = 0, msg = msg, count = totalRow, data = data };

            return JsonConvert.SerializeObject(result);
        }

        /// <summary>是否框架开发的租户</summary>
        /// <returns></returns>
        protected bool IsDeveloper()
        {
            return this.UserInfo.ClientId == AppConstManage.appDeveloper;
        }
        /// <summary>
        /// 是否框架功能。
        /// </summary>
        /// <returns></returns>
        protected bool IsDeveloperProg()
        {
            return this.SessionData.ProgInfoData.progInfo.ClientId == AppConstManage.appDeveloper;
        }
        #endregion

        #region 虚拟函数
        /// <summary>供子类重载</summary>
        protected virtual void PageLoadExt()
        {
            
        }
        //protected virtual void UserDevPageLoad()
        //{
            
        //}
        protected virtual void AppEventProcessExt(LibEventHandle ehandle)
        {
            
        }
        protected virtual void BeforeUpdate()
        {
            
        }

        protected virtual void AfterUpdate()
        {
            
        }

        #region grid相关
        protected virtual void TableActionExt(LibClientDatas clientDatas)
        {
            
        }
        protected virtual void GetTableActionExt(LibClientDatas clientDatas)
        {
            
        }
        #endregion

        #region search 相关
        /// <summary>设置搜索的字段</summary>
        /// <param name="tbnm">表名</param>
        /// <param name="kind">搜索种类（功能数据搜或来源主数据搜索）</param>
        /// <param name="fields">字段集合</param>
        protected virtual void SetSearchFieldsExt(string tbnm,LibSearchKind kind,ref List<LibFieldInfo> fields)
        {
            
        }

        /// <summary>搜索结果集处理</summary>
        /// <param name="tbnm">表名</param>
        /// <param name="kind">搜索种类（功能数据搜或来源主数据搜索）</param>
        /// <param name="data">结果集</param>
        protected virtual void SearchDataExt(string tbnm, LibSearchKind kind, ref List<object> data) { }
        #endregion

        #region 报表相关
        protected virtual void GetReportDataExt()
        {
            
        }
        #endregion

        #endregion


        #region 私有函数
        /// <summary>
        /// 转为对象
        /// </summary>
        /// <param name="clientDatas"></param>
        private void ConvertToobj(LibClientDatas clientDatas)
        {
            if (clientDatas != null && clientDatas.ClientDataInfos != null)
            {
                Type t = this.tDal.GetTypeBytModelNm(clientDatas.TableNm);
                foreach (LibClientDataInfo item in clientDatas.ClientDataInfos)
                {
                    item.Datas = LibAppUtils.JobjectToType(item, t);
                }
            }
        }
        #endregion

        #region IActionFilter 实现
        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (HttpContext != null && HttpContext.Request != null)
            {
                if (context.ActionArguments.ContainsKey("prog"))
                {
                    this.ProgNm = context.ActionArguments["prog"].ToString();
                }
                if (string.IsNullOrEmpty(this.ProgNm))
                    this.ProgNm = HttpContext.Request.Query["progNm"];
                if (string.IsNullOrEmpty(this.ProgNm))
                {
                    string[] arry = HttpContext.Request.Path.ToString().Split('/');
                    this.ProgNm = arry[arry.Length - 1];
                }
                this.ProgNm = this.ProgNm.ToUpper();
            }
            #region 用户登录验证
            if (context.RouteData.Values["controller"].ToString() != "Author") {
                if (this.UserInfo == null || string.IsNullOrEmpty(this.UserInfo.SessionId))
                {
                    //msg000000003 未登陆，请先登录
                    ThrowErrorException(3);
                }
            }
            #endregion 
            //this.ClientDatas = RedisHelper.Get<List<LibClientDatas>>(string.Format("{0}_{1}", HttpContext.Session.Id, this.ProgNm));
            //this.ClientDatas = HttpContext.Session.GetObject<List<LibClientDatas>>(string.Format("{0}_ClientData", this.ProgNm));
            //CachHelp cach = new CachHelp();
            this.ClientDatas =(List<LibClientDatas>)_cachHelp.GetCach(string.Format("{0}_{1}", HttpContext.Session.Id, this.ProgNm));
            if (this.ClientDatas == null)
            {
                //string key= string.Format("{0}_ClientData", this.ProgNm)
                //this.ClientDatas = HttpContext.Session.GetObject<List<LibClientDatas>>(this._key);
                this.ClientDatas = RedisHelper.Get<List<LibClientDatas>>(this._key);
                if (this.ClientDatas == null)
                {
                    this.ClientDatas = new List<LibClientDatas>();
                }
                else
                {
                    Type t;
                    foreach (LibClientDatas item in this.ClientDatas)
                    {
                        t = this.tDal.GetTypeBytModelNm(item.TableNm);
                        //if (t != null)
                        //{
                        foreach (LibClientDataInfo info in item.ClientDataInfos)
                        {
                            if (t != null)
                            {
                                if (info.Datas.GetType().Equals(typeof(JObject)))
                                {
                                    info.Datas = LibAppUtils.JobjectToType(info, t);
                                }
                                if (info.OldDatas != null && info.OldDatas.GetType().Equals(typeof(JObject)))
                                    info.OldDatas = LibAppUtils.JobjectToType(info.OldDatas, t);
                            }
                            else
                            {
                                //用户自定义的表
                                if (info.Datas.GetType().Equals(typeof(JObject)))
                                {
                                    info.Datas = JsonConvert.DeserializeObject<Dictionary<string, object>>(info.Datas.ToString());
                                }
                                if (info.OldDatas != null && info.OldDatas.GetType().Equals(typeof(JObject)))
                                    info.OldDatas = JsonConvert.DeserializeObject<Dictionary<string, object>>(info.OldDatas.ToString());
                            }
                        }
                        //}
                    }
                }
                _cachHelp.AddCachItem(string.Format("{0}_{1}", HttpContext.Session.Id, this.ProgNm), this.ClientDatas, TimeOffset, new LibClientDataChangeMonitor(this._key, this.ClientDatas, this.ProgNm, HttpContext));
            }

            this.tDal.ProgNm = this.ProgNm;
            this.tDal.Language = this.LibLanguage;
            this.tDal.UserInfo = this.UserInfo;
            //this.tDal.U_TBNm = this.SessionData.U_TBNm;
            //this.tDal.U_TBFieldNm = this.SessionData.U_TBFieldNm;
            #region
            #endregion 
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            if (this._sessiondata != null)
                HttpContext.Session.SetObject(this.ProgNm, this._sessiondata);
            if (this._userinfo != null)
                HttpContext.Session.SetObject(AppConstManage.appuserinfo, this._userinfo);
            //HttpContext.Session.SetObject(AppConstManage.applanguage, this._language);
            //RedisHelper.Set(string.Format("{0}_ClientData",this.ProgNm), this.ClientDatas,1200);
            //HttpContext.Session.SetObject(string.Format("{0}_ClientData", this.ProgNm), this.ClientDatas);
        
        }
        #endregion 
    }
}
