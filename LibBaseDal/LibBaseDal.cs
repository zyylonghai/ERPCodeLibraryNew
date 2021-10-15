using ErpCode.Com;
using ErpCode.Com.Enums;
using ErpModels;
using ErpModels.AppDataLog;
using ErpModels.Appsys;
using ErpModels.Com;
using LibDBContext;
using Library.Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LibraryBaseDal
{
    public partial class LibDataBaseDal<TDBContext> : LibBaseDal
        where TDBContext : BaseDBContext
    {
        #region 私有属性
        private TDBContext _dbcontext;
        private DateTimeOffset languageTimeOffset { get { return DateTime.Now.AddSeconds(1200); } }
        private List<object> _datas = null;
        delegate void WriteDataLogDelegate(List<object> datas);
        #endregion 
        #region 公开属性
        public TDBContext dBContext { get { return _dbcontext; } }
        public List<LibClientDatas> ClientDatas { get; set; }
        #endregion 

        #region 构造函数
        public LibDataBaseDal()
        {
            _datas = new List<object>();
            _dbcontext = Activator.CreateInstance<TDBContext>();
        }
        #endregion 

        #region 公开函数

        public string AppGetFieldDesc(string prognm, string tbnm, string fieldnm)
        {
            LanguageField v = null;
            CachHelp cachHelp = new CachHelp();
            List<LanguageField> data = (List<LanguageField>)cachHelp.GetCach(prognm);
            if (data == null || data.Count <= 0)
            {
                data = this.AppGetFieldDesc((int)this.Language, prognm, string.Empty, string.Empty);
                cachHelp.AddCachItem(prognm, data, languageTimeOffset);
                v = data.FirstOrDefault(i => i.TableNm.ToLower() == tbnm.ToLower() && i.FieldNm.ToLower() == fieldnm.ToLower());
                if (v == null)
                {
                    v= data.FirstOrDefault(i =>i.FieldNm.ToLower() == fieldnm.ToLower());
                }
                if (v != null)
                    return v.Vals;
            }
            v = data.FirstOrDefault(i => i.TableNm.ToLower() == tbnm.ToLower() && i.FieldNm.ToLower() == fieldnm.ToLower());
            if (v == null)
            {
                v = data.FirstOrDefault(i =>i.FieldNm.ToLower() == fieldnm.ToLower());
            }
            if (v != null)
                return v.Vals;
            else
            {
                data = (List<LanguageField>)cachHelp.GetCach(string.Empty);
                if (data == null || data.Count <= 0)
                {
                    data = this.AppGetFieldDesc((int)this.Language, string .Empty, string.Empty, string.Empty);
                    cachHelp.AddCachItem(string.Empty, data, languageTimeOffset);
                    v = data.FirstOrDefault(i => i.TableNm.ToLower() == tbnm.ToLower() && i.FieldNm.ToLower() == fieldnm.ToLower());
                    if(v==null)
                        v = data.FirstOrDefault(i =>i.FieldNm.ToLower() == fieldnm.ToLower());
                    if (v != null)
                        return v.Vals;
                }
                v = data.FirstOrDefault(i => i.TableNm.ToLower() == tbnm.ToLower() && i.FieldNm.ToLower() == fieldnm.ToLower());
                if (v == null)
                    v = data.FirstOrDefault(i => i.FieldNm.ToLower() == fieldnm.ToLower());
                if (v != null)
                    return v.Vals;
            }
            return fieldnm;
        }

        public void DetailHandle<T>(List<T> details)
            where T : LibModelCore
        {
            if (details != null && details.Count > 0)
            {
                foreach (T item in details)
                {
                    MastHandle(item);
                }
            }
        }

        public void MastHandle<T>(T Mast)
            where T : LibModelCore
        {
            if (Mast != null)
            {
                switch (Mast.LibModelStatus)
                {
                    case LibModelStatus.Add:
                        _dbcontext.Add(Mast);
                        Mast.CreateDT = System.DateTime.Now;
                        Mast.Creater = this.UserInfo.UserNm;
                        Mast.ClientId = this.UserInfo.ClientId;
                        break;
                    case LibModelStatus.Edit:
                    case LibModelStatus.Delete:
                        _dbcontext.Update(Mast);
                        Mast.LastModifyDT = System.DateTime.Now;
                        Mast.LastModifier = this.UserInfo.UserNm;
                        Mast.IsDeleted = Mast.LibModelStatus == LibModelStatus.Delete;
                        break;
                    case LibModelStatus.KeyUpdate:
                        _dbcontext.Add(Mast);
                        Mast.LastModifyDT = System.DateTime.Now;
                        Mast.LastModifier = this.UserInfo.UserNm;
                        Mast.ClientId = this.UserInfo.ClientId;
                        break;
                    //case LibModelStatus.Delete:
                    //    //_dbcontext.Remove(Mast);
                    //    Mast.LastModifyDT = System.DateTime.Now;
                    //    Mast.LastModifier = this.UserInfo.UserNm;
                    //    break;
                }
                this._datas.Add(Mast);
            }
        }

        public void DeleteHandle<T>(T data)
            where T : LibModelCore
        {
            if (data != null)
            {
                data.LibModelStatus = LibModelStatus.Delete;
                _dbcontext.Remove(data);
                //this._datas.Add(data);
            }
        }

        public void DeleteDetailsHandle<T>(List<T> details)
            where T : LibModelCore
        {
            if (details != null && details.Count > 0)
            {
                foreach (T item in details)
                {
                    DeleteHandle(item);
                }
            }
        }

        public void SaveChange()
        {
            _dbcontext.SaveChanges();
            DoWritDataLog(this._datas);
        }
        #endregion

        #region 受保护函数

        protected virtual void BeforeUpdate()
        {

        }
        //protected void AddMessage(string msg, LibMessageType type)
        //{
        //    this._MsgList.Add(new LibMessage { Message = msg, MsgType = type });
        //}
        #endregion 


        #region 私有函数
        private void ClientDataUpdateDBContext()
        {
            Type t = null;
            Type clientdatatype = null;
            object o;
            LibModelCore sys = null;
            //List<object> alldatas = new List<object>();
            foreach (LibClientDatas datas in ClientDatas)
            {
                t = GetModelType(datas.TableNm);

                foreach (LibClientDataInfo dtinfo in datas.ClientDataInfos)
                {
                    clientdatatype = dtinfo.Datas.GetType();
                    if (clientdatatype.Equals(typeof(JObject)))
                    {
                        o = LibAppUtils.JobjectToType(dtinfo, t);
                    }
                    else
                        o = dtinfo.Datas;
                    if (dtinfo.clientDataStatus == LibClientDataStatus.Add)
                    {
                        sys = (LibModelCore)o;
                        sys.CreateDT = DateTime.Now;
                        sys.Creater = "admin";
                        sys.ClientId = this.UserInfo.ClientId;
                        sys.LibModelStatus = LibModelStatus.Add;
                        MastHandle(sys);
                    }
                    else if (dtinfo.clientDataStatus == LibClientDataStatus.Edit)
                    {
                        sys = (LibModelCore)o;
                        if (dtinfo.OldDatas != null)
                        {
                            var old = dtinfo.OldDatas as LibModelCore;
                            old.LibModelStatus = LibModelStatus.Delete;
                            sys.LibModelStatus = LibModelStatus.KeyUpdate;
                            //MastHandle(old);
                            DeleteHandle(old);
                        }
                        else
                            sys.LibModelStatus = LibModelStatus.Edit;
                        sys.LastModifyDT = DateTime.Now;
                        sys.LastModifier = this.UserInfo.UserNm;
                        sys.ClientId = string.IsNullOrEmpty(sys.ClientId) ? this.UserInfo.ClientId : sys.ClientId;
                        //sys.LibModelStatus = LibModelStatus.Edit;
                        MastHandle(sys);
                    }
                    else if (dtinfo.clientDataStatus == LibClientDataStatus.Delete)
                    {
                        sys = (LibModelCore)o;
                        sys.LibModelStatus = LibModelStatus.Delete;
                        //DeleteHandle(sys);
                        MastHandle(sys);
                    }
                }
            }
            SaveChange();
        }
        protected async Task DoWritDataLog(List<object> datas,bool isudata=false)
        {
            await Task.Run(() =>
            {
                using (DataLogDBContext log = new DataLogDBContext())
                {
                    StringBuilder sqlbuilder = new StringBuilder();
                    LibModelCore obj = null;
                    foreach (object item in datas)
                    {
                        if (isudata)
                        {
                            LibClientDataInfo d = item as LibClientDataInfo;
                            sqlbuilder.Append(DoGetLogSqlStrForUdata(d.Datas, d.LogAction, d.TableNm, log));
                        }
                        else
                        {
                            obj = (LibModelCore)item;
                            if (obj.LibModelStatus == LibModelStatus.Add)
                            {
                                sqlbuilder.Append(DoGetLogSqlStr((LibModelCore)item, 1, log));
                            }
                            else if (obj.LibModelStatus == LibModelStatus.Edit || obj.LibModelStatus == LibModelStatus.KeyUpdate || (obj.LibModelStatus == LibModelStatus.Delete && obj.IsDeleted))
                            {
                                sqlbuilder.Append(DoGetLogSqlStr((LibModelCore)item, 2, log));
                            }
                        }
                    }
                    if (!string.IsNullOrEmpty(sqlbuilder.ToString()))
                        log.Database.ExecuteSql(sqlbuilder.ToString());
                }
            });
        }
        private void CallBackMethod(IAsyncResult ar)
        {

        }
        private string DoGetLogSqlStr(LibModelCore model, int action, DataLogDBContext logdb)
        {
            Type t = model.GetType();
            LibDbParameter[] parameters = new LibDbParameter[4];
            parameters[0] = new LibDbParameter { ParameterNm = "@logid", DbType = DbType.String, Value = model.app_logid };
            parameters[1] = new LibDbParameter { ParameterNm = "@tablenm", DbType = DbType.String, Value = t.Name };
            parameters[2] = new LibDbParameter { ParameterNm = "@ID", DbType = DbType.Int64, Direction = ParameterDirection.Output, Value = 0 };
            parameters[3] = new LibDbParameter { ParameterNm = "@logtbnm", DbType = DbType.String, Size = 35, Direction = ParameterDirection.Output, Value = string.Empty };
            logdb.Database.ExeStoredProcedure(action == 1 ? "p_addlogM" : "p_GetlogM", parameters);
            StringBuilder sql = new StringBuilder();
            PropertyInfo[] ps = t.GetProperties();
            if (!string.IsNullOrEmpty(parameters[3].Value.ToString()) && (Int64)parameters[2].Value != 0)
            {
                if (ps.Length > 0)
                {
                    //foreach (PropertyInfo p in ps)
                    //{
                    //if (p.Name == AppConstManage.applogid) continue;
                    sql.AppendFormat("  EXEC sp_executesql N'insert into {0}(ID,Action,UserId,IP,DT,Datajson) values(@ID,@Action,@UserId,@IP,@DT,@Datajson) '", parameters[3].Value.ToString());
                    sql.Append(" ,N'@ID bigint,@Action char(1),@UserId nvarchar(30),@IP nvarchar(15),@DT datetime,@Datajson ntext',");
                    sql.AppendFormat("  @ID={0},@Action='{1}',@UserId='{2}',@IP='{3}',@DT='{4}',@Datajson='{5}' ",
                                          (Int64)parameters[2].Value, action, this.UserInfo.UserNm, this.UserInfo.IP, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff"), JsonConvert.SerializeObject(model));
                    //}
                }
            }
            return sql.ToString();
        }

        /// <summary>
        /// 只用于自定义表数据的日志插入语法。
        /// </summary>
        /// <param name="model"></param>
        /// <param name="action"></param>
        /// <param name="tbnm"></param>
        /// <param name="logdb"></param>
        /// <returns></returns>
        private string DoGetLogSqlStrForUdata(object model,int action,string tbnm, DataLogDBContext logdb)
        {
            Dictionary<string, object> data = model as Dictionary<string, object>;
            LibDbParameter[] parameters = new LibDbParameter[4];
            parameters[0] = new LibDbParameter { ParameterNm = "@logid", DbType = DbType.String, Value = data[AppConstManage.applogid] };
            parameters[1] = new LibDbParameter { ParameterNm = "@tablenm", DbType = DbType.String, Value = tbnm };
            parameters[2] = new LibDbParameter { ParameterNm = "@ID", DbType = DbType.Int64, Direction = ParameterDirection.Output, Value = 0 };
            parameters[3] = new LibDbParameter { ParameterNm = "@logtbnm", DbType = DbType.String, Size = 35, Direction = ParameterDirection.Output, Value = string.Empty };
            logdb.Database.ExeStoredProcedure(action == 1 ? "p_addlogM" : "p_GetlogM", parameters);
            StringBuilder sql = new StringBuilder();
            //PropertyInfo[] ps = t.GetProperties();
            if (!string.IsNullOrEmpty(parameters[3].Value.ToString()) && (Int64)parameters[2].Value != 0)
            {
                //if (ps.Length > 0)
                //{
                    //foreach (PropertyInfo p in ps)
                    //{
                    //if (p.Name == AppConstManage.applogid) continue;
                    sql.AppendFormat("  EXEC sp_executesql N'insert into {0}(ID,Action,UserId,IP,DT,Datajson) values(@ID,@Action,@UserId,@IP,@DT,@Datajson) '", parameters[3].Value.ToString());
                    sql.Append(" ,N'@ID bigint,@Action char(1),@UserId nvarchar(30),@IP nvarchar(15),@DT datetime,@Datajson ntext',");
                    sql.AppendFormat("  @ID={0},@Action='{1}',@UserId='{2}',@IP='{3}',@DT='{4}',@Datajson='{5}' ",
                                          (Int64)parameters[2].Value, action, this.UserInfo.UserNm, this.UserInfo.IP, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff"), JsonConvert.SerializeObject(model));
                    //}
                //}
            }
            return sql.ToString();
        }

        private bool AppTable(string tbnm)
        {
            Type t = typeof(AppDBContext);
            PropertyInfo[] ps = t.GetProperties();
            return ps.FirstOrDefault(i => i.Name == tbnm) != null;
        }

        private string DoGenerateNo(CodeRuleConfig config, List<CodeRuleD> rules)
        {
            if (rules != null && rules.Count > 0)
            {
                string currdate = config.CurrDate;
                int currserial = config.CurrSerial;
                LibModule module;
                int serialen = -1;
                int index = 0;
                StringBuilder dateformat = new StringBuilder();
                StringBuilder format = new StringBuilder();
                foreach (var dr in rules)
                {
                    module = dr.ModuleId;
                    switch (module)
                    {
                        case LibModule.yyyy:
                        case LibModule.yy:
                        case LibModule.MM:
                        case LibModule.dd:
                            dateformat.Append(module);
                            format.Append(module);
                            break;
                        case LibModule.prefix:
                        case LibModule.suffix:
                            format.Append(dr.FixValue);
                            break;
                        case LibModule.serial:
                            format.Append("{" + index + "}");
                            index++;
                            serialen = Convert.ToInt32(dr.SeriaLen);
                            break;

                    }
                }
                if (serialen != -1)
                {
                    currserial = DateTime.Now.ToString(dateformat.ToString()) == currdate ? (currserial + 1) : 1;
                }
                #region 更新CodeRuleConfig表的当前日期和流水号
                config.CurrDate = DateTime.Now.ToString(dateformat.ToString());
                config.CurrSerial = currserial;
                #endregion 
                return DateTime.Now.ToString(string.Format(format.ToString(), currserial.ToString().PadLeft(serialen, '0')));
            }
            else
            {
                //msg000000008   找不到或未配置编码规则，请确认
                this.AddMessage(AppGetMessageDesc(8), LibMessageType.Error);
            }
            return string.Empty;
        }
        #endregion
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
            string message = AppGetMessageDesc(msgcode);
            ExceptionHelp help = new ExceptionHelp();
            help.ThrowError(this, string.Format(message, parms));
        }

        public string AppGetMessageDesc(int msgcode,params object[] parms)
        {
            string msg = AppGetFieldDesc(string.Empty, string.Empty, string.Format("msg{0}", msgcode.ToString().PadLeft(9, '0')));
            return string.Format(msg ,parms);
        }

        /// <summary>当前是框架开发的租户</summary>
        /// <returns></returns>
        public bool IsDeveloper()
        {
            return this.UserInfo.ClientId == AppConstManage.appDeveloper;
        }

        #region 锁相关函数
        /// <summary>
        /// 将LibDataLock类型锁，加入到锁容器中
        /// </summary>
        /// <typeparam name="T">被锁的数据对象类型需继承LibModelCore</typeparam>
        /// <param name="tbnm">锁钥匙（一般用表名）</param>
        /// <param name="data">被锁的数据对象</param>
        protected void AddDataLock<T>(string tbnm, T data)
            where T : LibModelCore
        {
            LockHelp<LibDataLock>.AddLock(tbnm, this.UserInfo.UserNm, data);
        }

        /// <summary>
        /// 将LibDataLock类型锁，从锁容器中移除
        /// </summary>
        /// <typeparam name="T">被锁的数据对象类型需继承LibModelCore</typeparam>
        /// <param name="tablenm">锁钥匙（一般用表名）</param>
        /// <param name="data">被锁的数据对象</param>
        protected void RemoveDataLock<T>(string tablenm, T data)
            where T : LibModelCore
        {
            List<LibDataLock> locks = LockHelp<LibDataLock>.GetLock(tablenm);
            if (locks != null)
            {
                LibDataLock l = locks.FirstOrDefault(i => i.Status == LibLockStatus.Lock && i.HasExist(data));
                if (l != null)
                {
                    LockHelp<LibDataLock>.RemoveLock(l);
                }
            }
        }

        protected bool ExistDataLock<T>(string tablenm, T data,out string clientid)
            where T : LibModelCore
        {
            List<LibDataLock> locks = LockHelp<LibDataLock>.GetLock(tablenm);
            if (locks != null)
            {
                var lk = locks.FirstOrDefault(i => i.Status == LibLockStatus.Lock && i.HasExist(data));
                if (lk != null)
                {
                    clientid = lk.ClientId;
                    return true;
                }
            }
            clientid = string.Empty;
            return false;
        }
        #endregion
        #region LibBaseDal 抽象方法实现
        public override void Update(List<LibClientDatas> clientdata)
        {
            //foreach (LibClientDatas item in clientdata)
            //{
            //    foreach (LibClientDataInfo info in item.ClientDataInfos)
            //    {
            //        info.Datas = JsonConvert.DeserializeObject(((JObject)info.Datas).ToString());
            //    }
            //}
            this.ClientDatas = clientdata;
            BeforeUpdate();
            ClientDataUpdateDBContext();
        }

        public override Type GetTypeBytModelNm(string tbnm)
        {
            return GetModelType(tbnm);
        }

        public override List<LanguageField> AppGetFieldDesc(int languageid, string prognm, string tbnm, string fieldnm)
        {
            string cltid = string.Empty;
            using (AppDBContext appDB = new AppDBContext())
            {
                if (this.UserInfo != null)
                    cltid = this.UserInfo.ClientId;
                if (string.IsNullOrEmpty(fieldnm) && !string.IsNullOrEmpty(tbnm))
                {
                    return appDB.LanguageField.Where(i => i.LanguageId == (LibLanguage)languageid && i.ProgNm == prognm && i.TableNm == tbnm && (i.ClientId == cltid || i.ClientId ==AppConstManage.appDeveloper)).ToList();
                }
                else if (string.IsNullOrEmpty(fieldnm) && string.IsNullOrEmpty(tbnm))
                {
                    return appDB.LanguageField.Where(i => i.LanguageId == (LibLanguage)languageid && i.ProgNm == prognm && (i.ClientId == cltid || i.ClientId == AppConstManage.appDeveloper)).ToList();
                }
                return appDB.LanguageField.Where(i => i.LanguageId == (LibLanguage)languageid && i.ProgNm == prognm && i.TableNm == tbnm && i.FieldNm == fieldnm && (i.ClientId == cltid || i.ClientId == AppConstManage.appDeveloper)).ToList();
            }
        }

        public override List<object> SearchData(string tablenm, WhereObject whereObject)
        {
            string sql = string.Empty;
            if (string.IsNullOrEmpty(whereObject.WhereFormat))
            {
                sql = string.Format("EXEC sp_executesql N'SELECT *From {0}'", tablenm);
            }
            else
                sql = string.Format("EXEC sp_executesql N'SELECT *From {0} where {1}',{2}", tablenm, whereObject.WhereFormat, whereObject.ValueTostring);
            if (AppTable(tablenm))
            {
                using (AppDBContext appdb = new AppDBContext())
                {
                    return appdb.Database.SqlQuery(sql).ToList(GetModelType(tablenm));
                }
            }
            return this.dBContext.Database.SqlQuery(sql).ToList(GetModelType(tablenm));
        }

        public override List<object> FillSearchData(Dictionary<string, object> mstKeys, string tbnm)
        {
            string sql = string.Empty;
            WhereObject whereObject = new WhereObject();
            whereObject.Values = new object[mstKeys.Count];
            StringBuilder whereformat = new StringBuilder();
            int n = 0;
            foreach (KeyValuePair<string, object> keyValue in mstKeys)
            {
                if (whereformat.Length > 0)
                {
                    whereformat.Append(" and ");
                }
                whereformat.Append("" + keyValue.Key + "={" + n + "}");
                whereObject.Values[n] = keyValue.Value;
                whereObject.WhereFormat = whereformat.ToString();
                n++;
            }
            sql = string.Format("EXEC sp_executesql N'SELECT *From {0} where {1}',{2}", tbnm, whereObject.WhereFormat, whereObject.ValueTostring);
            return this.dBContext.Database.SqlQuery(sql).ToList(GetModelType(tbnm));
        }

        public override List<DataLogsD> SearchDataLogs(string tbnm, string logid)
        {
            LibDbParameter[] parameters = new LibDbParameter[2];
            parameters[0] = new LibDbParameter { ParameterNm = "@tablenm", DbType = DbType.String, Value = tbnm };
            parameters[1] = new LibDbParameter { ParameterNm = "@logid", DbType = DbType.String, Value = logid };
            using (DataLogDBContext logdb = new DataLogDBContext())
            {
                return logdb.Database.ExeStoredProcedureToData("p_searclog", parameters).ToList<DataLogsD>();
            }
        }

        public override string GenerateNoByprogNm(string progNm)
        {
            using (ComDBContext comdb = new ComDBContext())
            {
                string result = string.Empty;
                var rules = (from a in comdb.CodeRuleConfig
                             join b in comdb.CodeRule on a.RuleId equals b.RuleId
                             join c in comdb.CodeRuleD on b.RuleId equals c.RuleId
                             where a.ProgNm == progNm && b.Status == LibStatus.Enable && a.IsDefault
                             select new { a, c }).ToList();
                var config = rules.Select(i => i.a).FirstOrDefault();
                var ruledt = rules.Select(i => i.c).ToList();

                try
                {
                    #region 判断是否有加锁
                    if (this.ExistDataLock("CodeRuleConfig", config, out string clientid))
                    {
                        //msg00000009  该功能的规则编号生成，正被用户{0}锁着。
                        this.AddMessage(this.AppGetMessageDesc(9,clientid), LibMessageType.Error);
                    }
                    #endregion
                    #region 加锁，防止生成重复序列号
                    this.AddDataLock("CodeRuleConfig", config);
                    #endregion

                    result = DoGenerateNo(config, ruledt);
                    comdb.SaveChanges();
                }
                catch (Exception ex)
                {

                }
                finally
                {
                    #region 从容器中移除锁
                    RemoveDataLock("CodeRuleConfig", config);
                    #endregion
                }
                return result;
            }
        }

        #endregion
    }

    public abstract class LibBaseDal
    {
        private List<LibMessage> _MsgList = null;
        public List<LibMessage> Messages { get { return _MsgList; } }
        public string ProgNm { get; set; }
        public LibLanguage Language { get; set; }
        public LibClientUserInfo UserInfo { get; set; }

        /// <summary>用于存储自定义表信息数据存储的所在实际表名</summary>
        //public string U_TBNm { get; set; }

        ///// <summary>用于存储自定义表字段信息数据存储的所在实际表名</summary>
        //public string U_TBFieldNm { get; set; }
        public abstract void Update(List<LibClientDatas> clientdata);
        public abstract List<object> SearchData(string tablenm, WhereObject whereObject);
        public abstract List<object> FillSearchData(Dictionary<string, object> mstKeys, string tbnm);
        public abstract List<DataLogsD> SearchDataLogs(string tbnm, string logid);
        public abstract string GenerateNoByprogNm(string progNm);

        public abstract Type GetTypeBytModelNm(string tbnm);
        public abstract List<LanguageField> AppGetFieldDesc(int languageid, string prognm, string tbnm, string fieldnm);
        public void AddMessage(string msg, LibMessageType type)
        {
            if (_MsgList == null) _MsgList = new List<LibMessage>();
            this._MsgList.Add(new LibMessage { Message = msg, MsgType = type });
        }

    }
}
