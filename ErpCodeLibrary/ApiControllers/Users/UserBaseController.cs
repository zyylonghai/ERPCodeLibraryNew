using ErpCode.BaseApiController;
using ErpCode.Com;
using ErpCode.Com.Enums;
using ErpCode.UserDal;
using ErpModels.Appsys;
using ErpModels.UserTable;
using Library.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ErpCodeLibrary.ApiControllers.Users
{
    [Route("Users/[action]/{prog}")]
    [ApiController]
    [Authorize]
    public class UserBaseController : DataBaseController<UserDal>
    {
        public override ResponseMsg PageLoaded()
        {
            LibClientDatas clientDatas = null;
            LibClientDataInfo dataInfo = null;
            //#region 获取自定义的存储位置
            //var storageinfo = this.tDal.GetStorageInfo(this.UserInfo.ClientId);
            //if (storageinfo != null)
            //{
            //    this.SessionData.U_TBFieldNm = storageinfo.StorageTableFieldNm;
            //    this.SessionData.U_TBNm = storageinfo.StorageTableNm;
            //}
            //#endregion 
            if (string.Compare(this.ProgNm, AppConstManage.appUserDefindTable, true) == 0)
            {
                return base.PageLoaded();
            }
            if (this.SessionData.u_TableFieldInfos == null) this.SessionData.u_TableFieldInfos = new List<U_TableFieldInfo>();
            if (this.OperatAction == LibOperatAction.Add || this.OperatAction == LibOperatAction.None)
            {
                this.ClientDatas.Clear();
                if (this.SessionData.ProgInfoData != null)
                {
                    foreach (var item in this.SessionData.ProgInfoData.progControlInfos)
                    {
                        //List<U_TableFieldInfo> tbfs = this.SessionData.u_TableFieldInfos.Where(i => i.TableNm == item.TableNm).ToList();
                        //if (tbfs == null || tbfs.Count <= 0)
                        //{
                        //    tbfs = this.tDal.GetTableFieldsInfo(item.TableNm);
                        //    this.SessionData.u_TableFieldInfos.AddRange(tbfs);
                        //}
                        List<U_TableFieldInfo> tbfs = GetU_TableFieldInfos(item.TableNm);
                        if (tbfs != null && tbfs.Count > 0)
                        {
                            clientDatas = new LibClientDatas();
                            clientDatas.TableNm = item.TableNm;
                            clientDatas.DataSource = item.DataSourceNm;
                            if (item.ControlType == LibControlType.Colla)
                            {
                                clientDatas.collas.Add(item.ID);
                                dataInfo = new LibClientDataInfo();
                                dataInfo.clientDataStatus = LibClientDataStatus.Add;
                                dataInfo.PrimaryKeyInfos = new List<LibPrimaryKeyInfo>();
                                Dictionary<string, object> row = new Dictionary<string, object>();
                                foreach (var r in tbfs)
                                {
                                    if (r.IsPrimaryKey)
                                    {
                                        dataInfo.PrimaryKeyInfos.Add(new LibPrimaryKeyInfo { KeyColumn = r.FieldNm, Value = string.Empty });
                                    }
                                    row.Add(r.FieldNm, string.Empty);
                                }
                                if (!row.ContainsKey(AppConstManage.applogid))
                                    row.Add(AppConstManage.applogid, string.Empty);
                                dataInfo.Datas = row;
                                clientDatas.ClientDataInfos.Add(dataInfo);
                                #region 解析默认值表达式
                                var fs = this.SessionData.ProgInfoData.progFieldInfos.Where(i => i.ID == item.ID && !i.IsHide && !string.IsNullOrEmpty(i.DefaultValue.Trim()));
                                if (fs != null)
                                {
                                    foreach (var f in fs)
                                    {
                                        string[] expesslist = f.DefaultValue.Split(';');
                                        if (expesslist != null)
                                        {
                                            var o = tbfs.FirstOrDefault(i => i.FieldNm == f.Field);
                                            if (o == null) continue;
                                            foreach (string exp in expesslist)
                                            {
                                                if (exp.StartsWith("ConstVal", StringComparison.OrdinalIgnoreCase))//赋常量
                                                {
                                                    ////if (o != null)
                                                    ////{
                                                    string v = LibAppUtils.GetBracketContent(exp, BracketType.Parentheses);
                                                    v = v.Replace("\"", "");
                                                    //if (o.PropertyType.Equals(typeof(decimal)) || o.PropertyType.Equals(typeof(int)) ||
                                                    //    o.PropertyType.Equals(typeof(double)) || o.PropertyType.Equals(typeof(long)))
                                                    //{
                                                    //    o.SetValue(dataInfo.Datas, Convert.ToDecimal(v));
                                                    //}
                                                    //else
                                                    //    o.SetValue(dataInfo.Datas, v);
                                                    ////}
                                                }
                                                else if (exp.StartsWith("GetCurrentDate", StringComparison.OrdinalIgnoreCase))//取当前日期
                                                {
                                                    //if (o.PropertyType.Equals(typeof(DateTime)))
                                                    //{
                                                    //    o.SetValue(dataInfo.Datas, Convert.ToDateTime(System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")));
                                                    //}
                                                }
                                            }
                                        }
                                    }
                                }
                                #endregion
                            }
                            //else
                            //{
                            //    #region 收集主键字段

                            //    #endregion
                            //}
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
            }

            var rsp = ResponseMsg;
            rsp.Data = this.OperatAction;
            //rsp.Message = "has PageLoaded";
            return rsp;
        }
        //protected override void UserDevPageLoad()
        //{
        //    base.UserDevPageLoad();
        //    LibClientDatas clientDatas = null;
        //    LibClientDataInfo dataInfo = null;
        //    if (this.SessionData.u_TableFieldInfos == null) this.SessionData.u_TableFieldInfos = new List<U_TableFieldInfo>();
        //    if (this.OperatAction == LibOperatAction.Add || this.OperatAction == LibOperatAction.None)
        //    {
        //        this.ClientDatas.Clear();
        //        if (this.SessionData.ProgInfoData != null)
        //        {
        //            foreach (var item in this.SessionData.ProgInfoData.progControlInfos)
        //            {
        //                List<U_TableFieldInfo> tbfs = this.SessionData.u_TableFieldInfos.Where(i => i.TableNm == item.TableNm).ToList();
        //                if (tbfs == null || tbfs.Count <= 0)
        //                {
        //                    tbfs = this.tDal.GetTableFieldsInfo(item.TableNm);
        //                    this.SessionData.u_TableFieldInfos.AddRange(tbfs);
        //                }
                        
        //                if (tbfs != null && tbfs.Count > 0)
        //                {
        //                    clientDatas = new LibClientDatas();
        //                    clientDatas.TableNm = item.TableNm;
        //                    clientDatas.DataSource = item.DataSourceNm;
        //                    if (item.ControlType == LibControlType.Colla)
        //                    {
        //                        clientDatas.collas.Add(item.ID);
        //                        dataInfo = new LibClientDataInfo();
        //                        dataInfo.clientDataStatus = LibClientDataStatus.Add;
        //                        Dictionary<string, object> row = new Dictionary<string, object>();
        //                        foreach (var r in tbfs)
        //                        {
        //                            row.Add(r.FieldNm, string.Empty);
        //                        }
        //                        dataInfo.Datas = row;
        //                        clientDatas.ClientDataInfos.Add(dataInfo);
        //                        #region 解析默认值表达式
        //                        var fs = this.SessionData.ProgInfoData.progFieldInfos.Where(i => i.ID == item.ID && !i.IsHide && !string.IsNullOrEmpty(i.DefaultValue.Trim()));
        //                        if (fs != null)
        //                        {
        //                            foreach (var f in fs)
        //                            {
        //                                string[] expesslist = f.DefaultValue.Split(';');
        //                                if (expesslist != null)
        //                                {
        //                                    var  o = tbfs.FirstOrDefault(i => i.FieldNm == f.Field);
        //                                    if (o == null) continue;
        //                                    foreach (string exp in expesslist)
        //                                    {
        //                                        if (exp.StartsWith("ConstVal", StringComparison.OrdinalIgnoreCase))//赋常量
        //                                        {
        //                                            ////if (o != null)
        //                                            ////{
        //                                            string v = LibAppUtils.GetBracketContent(exp, BracketType.Parentheses);
        //                                            v = v.Replace("\"", "");
        //                                            //if (o.PropertyType.Equals(typeof(decimal)) || o.PropertyType.Equals(typeof(int)) ||
        //                                            //    o.PropertyType.Equals(typeof(double)) || o.PropertyType.Equals(typeof(long)))
        //                                            //{
        //                                            //    o.SetValue(dataInfo.Datas, Convert.ToDecimal(v));
        //                                            //}
        //                                            //else
        //                                            //    o.SetValue(dataInfo.Datas, v);
        //                                            ////}
        //                                        }
        //                                        else if (exp.StartsWith("GetCurrentDate", StringComparison.OrdinalIgnoreCase))//取当前日期
        //                                        {
        //                                            //if (o.PropertyType.Equals(typeof(DateTime)))
        //                                            //{
        //                                            //    o.SetValue(dataInfo.Datas, Convert.ToDateTime(System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")));
        //                                            //}
        //                                        }
        //                                    }
        //                                }
        //                            }
        //                        }
        //                        #endregion
        //                    }
        //                    var exit = this.ClientDatas.FirstOrDefault(i => i.DataSource == item.DataSourceNm && i.TableNm == item.TableNm);
        //                    if (exit == null)
        //                    {
        //                        clientDatas.CollectIdentity(this.tDal.GetTypeBytModelNm(clientDatas.TableNm));
        //                        this.ClientDatas.Add(clientDatas);

        //                    }
        //                    else
        //                    {
        //                        exit.collas.Add(item.ID);
        //                    }
        //                }
        //            }
        //        }
        //    }
        //}

        public override ResponseMsg Update(List<LibClientDatas> clientdata)
        {
            //var t = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(clientdata[0].ClientDataInfos[0].Datas.ToString());
            if (string.Compare(this.ProgNm, AppConstManage.appUserDefindTable, true) == 0)
            {
                return base.Update(clientdata);
            }
            foreach (LibClientDatas item in clientdata)
            {
                var data = this.ClientDatas.FirstOrDefault(i => i.DataSource == item.DataSource && i.TableNm == item.TableNm);
                //if (data!=null && (data.TableNm == "ProgControlInfo" || data.TableNm == "ProgFieldInfo")) continue;
                foreach (LibClientDataInfo info in item.ClientDataInfos)
                {
                    Dictionary<string ,object > infodata = JsonConvert.DeserializeObject<Dictionary<string, object>>(info.Datas.ToString());
                    if (data != null && data.ClientDataInfos != null)
                    {
                        Dictionary<string, object> v = null;
                        foreach (LibClientDataInfo d in data.ClientDataInfos)
                        {
                            if (d.Datas.GetType().Equals(typeof(JObject)) || d.Datas.GetType().Equals(typeof(JsonElement)))
                            {
                                v = JsonConvert.DeserializeObject<Dictionary<string, object>>(d.Datas.ToString());
                            }
                            else
                                v = d.Datas as Dictionary<string, object>;
                            foreach (var keyval in infodata)
                            {
                                v[keyval.Key]= keyval.Value;
                            }
                            if (string.IsNullOrEmpty(v[AppConstManage.applogid].ToString()))
                            {
                                v[AppConstManage.applogid] = TimestampID.GetInstance().GetID();
                            }
                            d.clientDataStatus = d.clientDataStatus == LibClientDataStatus.Preview ? LibClientDataStatus.Edit : d.clientDataStatus;
                        }
                    }

                }
                if (data == null)
                    this.ClientDatas.Add(item);
            }
            this.tDal.Update(this.ClientDatas);
            AfterUpdate();
            //msg000000001 保存成功
            this.AddMessage(1, LibMessageType.Prompt);
            this.SessionData.OperatAction = LibOperatAction.Preview;
            this.AcceptClientData();
            return this.ResponseMsg;
        }

        #region 搜索 相关

        /// <summary></summary>
        /// <param name="tbnm"></param>
        /// <param name="flag">1 标识功能搜索，2 标识来源主数据搜索</param>
        /// <returns></returns>
        public override ResponseMsg GetSearchfieldandsymbol(string tbnm, int flag)
        {
            if (string.Compare(this.ProgNm, AppConstManage.appUserDefindTable, true) == 0)
            {
                return base.GetSearchfieldandsymbol(tbnm,flag);
            }
            List<LibFieldInfo> libFields = new List<LibFieldInfo>();
            string mtb = tbnm;
            Dictionary<string, List<LibEnumkeyvalue>> optionfields = new Dictionary<string, List<LibEnumkeyvalue>>();
            if (flag == 1) {
                var fields = this.SessionData.u_TableFieldInfos.Where(i => i.TableNm == this.SessionData.ProgInfoData.progInfo.mastTable);
                if (fields == null || fields.Count() <= 0)
                {
                    fields = this.tDal.GetTableFieldsInfo(this.SessionData.ProgInfoData.progInfo.mastTable);
                }
                if (fields != null && fields.Count() > 0)
                {
                    LibFieldInfo fieldInfo = null;
                    foreach (var f in fields)
                    {
                        fieldInfo = new LibFieldInfo { fieldNm = f.FieldNm, fieldDesc = AppGetFieldDesc(f.TableNm, f.FieldNm) };

                        libFields.Add(fieldInfo);
                    }
                }
            }
            //SetSearchFieldsExt(tbnm, (LibSearchKind)flag, ref libFields);
            List<LibEnumkeyvalue> symbols = LibAppUtils.GetenumFields<SmodalSymbol>();
            symbols.ForEach(i => { i.value = AppGetFieldDesc(string.Empty, "SmodalSymbol", i.value); });
            List<LibEnumkeyvalue> logic = LibAppUtils.GetenumFields<Smodallogic>();
            logic.ForEach(i => { i.value = AppGetFieldDesc(string.Empty, "", i.value); });
            ResponseMsg response = ResponseMsg;
            response.Data = new { fields = libFields, symbol = symbols, logic = logic, optionfields = optionfields };
            return response;
        }

        public override string BindSearchDataGrid(string tbnm, LibSearchKind kind, int limit, int page)
        {
            List<LibSearchCondition> conds = this.SessionData.GetDataExt<List<LibSearchCondition>>();
            object[] values = { };
            string tb = tbnm;
            List<object> data = null;
            if (string.Compare(this.ProgNm, AppConstManage.appUserDefindTable, true) == 0)
            {
                conds.Add(new LibSearchCondition { FieldNm = "ClientId", Logic = Smodallogic.And, Symbol = SmodalSymbol.Equal, valu1 = this.UserInfo.ClientId });
                StringBuilder whereformat = new StringBuilder();
                SearchConditionHelper.AnalyzeSearchCondition(conds, whereformat, ref values);
                WhereObject where = new WhereObject { WhereFormat = whereformat.ToString(), Values = values };
                tb = this.UserInfo.U_TBNm;
                if (string.IsNullOrEmpty(tb))
                    tb = this.tDal.GetTableInfoNm(this.UserInfo.ClientId);
                data = this.tDal.SearchData(tb, where);
            }
            else
            {
                data = this.tDal.SearchUData(conds, this.SessionData.ProgInfoData.progInfo.mastTable);
            }
            //SearchDataExt(string.IsNullOrEmpty(tbnm) ? this.SessionData.ProgInfoData.progInfo.mastTable : tbnm, kind, ref data);
            var result = new { code = 0, msg = "success", count = data.Count, data = data };
            return JsonConvert.SerializeObject(result);
        }

        public override ResponseMsg FillSearchData(object o)
        {
            if (string.Compare(this.ProgNm, AppConstManage.appUserDefindTable, true) == 0)
                return base.FillSearchData(o);
            else
            {
                this.SessionData.OperatAction = LibOperatAction.Preview;
                Dictionary<string, object> row = JsonConvert.DeserializeObject<Dictionary<string, object>>(o.ToString());
                Dictionary<string, object> keys = new Dictionary<string, object>();
                var fields = this.SessionData.u_TableFieldInfos.Where(i => i.TableNm == this.SessionData.ProgInfoData.progInfo.mastTable);
                foreach (var f in fields)
                {
                    if (f.IsPrimaryKey)
                    {
                        keys.Add(f.FieldNm, row[f.FieldNm]);
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
            }
            return ResponseMsg;
        }
        #endregion

        #region 表格相关
        public override ResponseMsg GetTableAction(LibClientDatas clientDatas)
        {
            if (string.Compare(this.ProgNm, AppConstManage.appUserDefindTable, true) == 0)
                return base.GetTableAction(clientDatas);
            else
            {
                ResponseMsg response = ResponseMsg;
                if (clientDatas.ClientDataInfos != null)
                {
                    Dictionary<string, object> row = null;
                    object v=null;
                    var fields = this.SessionData.u_TableFieldInfos.Where(i => i.TableNm == clientDatas.TableNm);
                    foreach (var item in clientDatas.ClientDataInfos)
                    {
                        row = JsonConvert.DeserializeObject<Dictionary<string, object>>(item.Datas.ToString());
                        if (fields != null)
                        {
                            foreach (var f in fields)
                            {
                                if (!row.TryGetValue(f.FieldNm,out v))
                                {
                                    row.Add(f.FieldNm, null);
                                }
                            }
                        }
                        if (!row.TryGetValue(AppConstManage.applogid, out v))
                        {
                            row.Add(AppConstManage.applogid, v);
                        }
                        if (string.IsNullOrEmpty(v.ToString()))
                        {
                            row[AppConstManage.applogid] = TimestampID.GetInstance().GetID();
                        }
                        item.Datas = row;
                    }
                }
                response.ClientDatas = new List<LibClientDatas>();
                response.ClientDatas.Add(clientDatas);
                return response;
            }
        }

        public override ResponseMsg TableAction(LibClientDatas clientDatas)
        {
            if (string.Compare(this.ProgNm, AppConstManage.appUserDefindTable, true) == 0)
                return base.TableAction(clientDatas);
            else
            {
                if (this.ClientDatas != null)
                {
                    foreach (var info in clientDatas.ClientDataInfos)
                    {
                        info.Datas = JsonConvert.DeserializeObject<Dictionary<string, object>>(info.Datas.ToString());
                    }
                    var exit = this.ClientDatas.FirstOrDefault(i => i.DataSource == clientDatas.DataSource && i.TableNm == clientDatas.TableNm);
                    if (exit != null)
                    {

                        if (exit.ClientDataInfos == null) exit.ClientDataInfos = new List<LibClientDataInfo>();
                        if (exit.ClientDataInfos.Count == 0)
                        {
                            exit.ClientDataInfos.AddRange( clientDatas.ClientDataInfos);
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

                                    Dictionary<string ,object> mcore = info.Datas as Dictionary<string, object>;
                                    foreach (var v in exit.ClientDataInfos)
                                    {
                                        Dictionary<string, object> tmcore = v.Datas as Dictionary<string, object>;
                                        if (tmcore[AppConstManage .applogid].ToString () == mcore[AppConstManage.applogid].ToString ())
                                        {
                                            if (v.clientDataStatus == LibClientDataStatus.Edit)
                                            {
                                                #region 找主键列
                                                if (v.PrimaryKeyInfos == null) v.PrimaryKeyInfos = new List<LibPrimaryKeyInfo>();
                                                v.PrimaryKeyInfos.Clear();
                                                bool updatekey = false;
                                                var primarykeys = this.SessionData.u_TableFieldInfos.Where(i => i.TableNm == clientDatas.TableNm && i.IsPrimaryKey);
                                                if (primarykeys != null)
                                                {
                                                    foreach (var key in primarykeys)
                                                    {
                                                        if (mcore[key.FieldNm].ToString() != tmcore[key.FieldNm].ToString())
                                                        {
                                                            updatekey = true;
                                                        }
                                                        v.PrimaryKeyInfos.Add(new LibPrimaryKeyInfo { KeyColumn = key.FieldNm, Value = tmcore[key.FieldNm] });
                                                    }
                                                }

                                                #endregion
                                                if (updatekey)
                                                    v.OldDatas = LibAppUtils.CopyFrom(tmcore);
                                                else
                                                    v.PrimaryKeyInfos = null;
                                            }
                                            LibAppUtils.CopyDictions(mcore, tmcore);
                                            v.Datas =tmcore;
                                            v.clientDataStatus = v.clientDataStatus == LibClientDataStatus.Add ? v.clientDataStatus : info.clientDataStatus;
                                        }
                                    }
                                    //TableActionExt(exit);
                                }
                                else if (info.clientDataStatus == LibClientDataStatus.Delete)
                                {
                                    Dictionary<string, object> mcore = info.Datas as Dictionary<string, object>;
                                    foreach (var v in exit.ClientDataInfos)
                                    {
                                        Dictionary<string, object> tmcore = v.Datas as Dictionary<string, object>;
                                        if (mcore[AppConstManage.applogid].ToString () == tmcore[AppConstManage.applogid].ToString ())
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
                    }
                }
                return ResponseMsg;
            }
        }
        #endregion

        #region 私有函数
        private List<U_TableFieldInfo> GetU_TableFieldInfos(string tbnm)
        {
            List<U_TableFieldInfo> tbfs = this.SessionData.u_TableFieldInfos.Where(i => i.TableNm == tbnm).ToList();
            if (tbfs == null || tbfs.Count <= 0)
            {
                tbfs = this.tDal.GetTableFieldsInfo(tbnm);
                this.SessionData.u_TableFieldInfos.AddRange(tbfs);
            }
            return tbfs;
        }
        #endregion 

    }
}
