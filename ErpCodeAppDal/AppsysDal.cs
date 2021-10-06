using ErpCode.Com;
using ErpCode.Com.Enums;
using ErpModels.Appsys;
using LibDBContext;
using LibraryBaseDal;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace ErpCode.AppDal
{
    public class AppsysDal: LibDataBaseDal<AppDBContext>
    {
        /// <summary>获取多语言数据</summary>
        /// <param name="prog">功能名</param>
        /// <param name="tbnm">表名</param>
        /// <param name="fieldnm">字段名</param>
        /// <param name="val">多语言值</param>
        /// <param name="limit">每页条数</param>
        /// <param name="page">页码</param>
        /// <returns></returns>
        public List<object> GetLanguageData(string prog, string tbnm, string fieldnm, string val, int limit, int page, out int totalrow)
        {
            List<object> data = new List<object>();
            List<LanguageField> list = null;
            //List<LanguageField> all = null;
            //int total = 0;
            prog = string.IsNullOrEmpty(prog) ? string.Empty : prog;
            tbnm = string.IsNullOrEmpty(tbnm) ? string.Empty : tbnm;
            fieldnm = string.IsNullOrEmpty(fieldnm) ? string.Empty : fieldnm;
            val = string.IsNullOrEmpty(val) ? string.Empty : val;
            if (string.IsNullOrEmpty(prog) && string.IsNullOrEmpty(tbnm) && string.IsNullOrEmpty(fieldnm) && string.IsNullOrEmpty(val))
            {
                var query = this.dBContext.LanguageField.AsEnumerable();
                totalrow = query.Count();
                list = query.Skip(limit * (page - 1)).Take(limit).ToList();
            }
            else
            {
                var query = this.dBContext.LanguageField.AsQueryable();
                if (!string.IsNullOrEmpty(prog))
                    query = query.Where(i => i.ProgNm == prog);
                if (!string.IsNullOrEmpty(tbnm))
                    query = query.Where(i => i.TableNm == tbnm);
                if (!string.IsNullOrEmpty(fieldnm))
                {
                    query = query.Where(i => i.FieldNm == fieldnm);
                }
                if (!string.IsNullOrEmpty(val))
                    query = query.Where(i => i.Vals.Contains(val));
                //all = query.ToList();
                totalrow = query.Count();
                list = query.Skip(limit * (page - 1)).Take(limit).ToList();
            }
            if (list != null)
            {
                var result = LibAppUtils.GetenumFields<LibLanguage>();
                //string jsonstr = Newtonsoft.Json.JsonConvert.SerializeObject(list);
                string key = string.Empty;
                JArray jArray = new JArray();
                JObject jObject = null;
                Dictionary<string, JObject> datadic = new Dictionary<string, JObject>();
                foreach (var item in list)
                {
                    key = string.Format("{0}-{1}-{2}", item.ProgNm, item.TableNm, item.FieldNm);
                    if (!datadic.TryGetValue(key, out jObject))
                    {
                        jObject = new Newtonsoft.Json.Linq.JObject();
                        jObject["progNm"] = item.ProgNm;
                        jObject["progDesc"] = item.ProgNm;
                        jObject["TableNm"] = item.TableNm;
                        jObject["FieldNm"] = item.FieldNm;
                        jArray.Add(jObject);
                        datadic.Add(key, jObject);
                    }
                    var exit= result.FirstOrDefault(i => i.key ==(int)item.LanguageId);
                    if (exit != null)
                    {
                        jObject[exit.value] = item.Vals;
                    }
                }
                data = JsonConvert.DeserializeObject<List<object>>(jArray.ToString());
            }
            //totalrow = totalrow ;
            return data;
        }

        /// <summary>注册租户</summary>
        /// <param name="license"></param>
        /// <returns>1:注册成功，0 注册失败</returns>
        public int TenantRegist(License license)
        {
            license.LibModelStatus = ErpModels.LibModelStatus.Add;
            TenantKeyInfo keyInfo = new TenantKeyInfo();
            keyInfo.ClientId = license.ClientId;
            keyInfo.Privatekey = license.Privatekey;
            keyInfo.PublicKey = license.PublicKey;
            keyInfo.LibModelStatus = ErpModels.LibModelStatus.Add;
            keyInfo.CreateDT = DateTime.Now;
            keyInfo.Creater = this.UserInfo.UserNm;

            license.CreateDT = DateTime.Now;
            license.Creater = this.UserInfo.UserNm;
            this.dBContext.Add(license);
            this.dBContext.Add(keyInfo);
            //this.MastHandle(keyInfo);
            //this.MastHandle(license);
            this.SaveChange();
            AddTableStorageInfo(license.ClientId);
            return 1;
        }
        public string GenerateClientId()
        {
            var o = this.dBContext.TenantKeyInfo.OrderBy(i => i.ClientId).LastOrDefault();
            return (int.Parse(o.ClientId) + 1).ToString();
        }

        /// <summary>获取租户自定义的数据源</summary>
        /// <returns></returns>
        public List<LibDataSource> GetUserDataSource()
        {
            List<LibDataSource> result = new List<LibDataSource>();
            using (UserTableDBContext udb = new UserTableDBContext())
            {
                var ds = udb.U_DataSourceInfo.Where(i => i.ClientId == this.UserInfo.ClientId && !i.IsDeleted).ToList();
                foreach (var item in ds)
                {
                    result.Add(new LibDataSource { dsNm = item.DataSourceNm, dsDesc = item.DataSourceDesc });
                }
            }
            return result;
        }

        /// <summary>获取租户数据源下的表信息</summary>
        /// <param name="ds"></param>
        /// <returns></returns>
        public List<LibTableInfo> GetUserTableInfos(string ds)
        {
            List<LibTableInfo> result = new List<LibTableInfo>();
            using (UserTableDBContext udb = new UserTableDBContext())
            {
                LibDbParameter[] parameters = new LibDbParameter[2];
                parameters[0] = new LibDbParameter { ParameterNm = "@clientid", DbType = DbType.String, Value = this.UserInfo.ClientId };
                parameters[1] = new LibDbParameter { ParameterNm = "@datasource", DbType = DbType.String, Value = ds };
                var tbs = udb.Database.ExeStoredProcedureToData("p_GetUserTableInfoByds", parameters);
                //var tbs = udb.U_TableInfo.Where(i =>i.DataSourceNm ==ds && i.ClientId == this.UserInfo.ClientId && !i.IsDeleted).ToList();
                foreach (DataRow item in tbs.Rows)
                {
                    result.Add(new LibTableInfo { tableNm=item["TableNm"].ToString(),tableDesc=item["TableDesc"].ToString()});
                }
            }
            return result;
        }
        /// <summary>
        /// 获取租户表下的字段信息
        /// </summary>
        /// <param name="tbnm"></param>
        /// <returns></returns>
        public List<LibFieldInfo> GetUserTableFieldinfo(string tbnm)
        {
            List<LibFieldInfo> result = new List<LibFieldInfo>();
            using (UserTableDBContext udb = new UserTableDBContext())
            {
                LibDbParameter[] parameters = new LibDbParameter[2];
                parameters[0] = new LibDbParameter { ParameterNm = "@clientid", DbType = DbType.String, Value = this.UserInfo.ClientId };
                parameters[1] = new LibDbParameter { ParameterNm = "@tbnm", DbType = DbType.String, Value = tbnm };
                var tbs = udb.Database.ExeStoredProcedureToData("p_GetUserTableFieldInfoBytbnm", parameters);
                //var tbs = udb.U_TableFieldInfo.Where(i => i.TableNm  == tbnm && i.ClientId == this.UserInfo.ClientId && !i.IsDeleted).ToList();
                foreach (DataRow item in tbs.Rows)
                {
                    result.Add(new LibFieldInfo { fieldNm = item["FieldNm"].ToString (), fieldDesc = item["FieldDesc"].ToString(), fieldType = (LibFieldType)item["DataType"] });
                }
            }
            return result;
        }

        public void AddTableStorageInfo(string clientid)
        {
            using (UserTableDBContext udb = new UserTableDBContext())
            {
                LibDbParameter[] parameters = new LibDbParameter[7];
                parameters[0] = new LibDbParameter { ParameterNm = "@clientid", DbType = DbType.String, Value = clientid };
                parameters[1] = new LibDbParameter { ParameterNm = "@CreateDT", DbType = DbType.DateTime, Value = DateTime.Now };
                parameters[2] = new LibDbParameter { ParameterNm = "@Creater", DbType = DbType.String, Value = this.UserInfo.UserNm };
                parameters[3] = new LibDbParameter { ParameterNm = "@logid", DbType = DbType.String, Value = string.Empty };
                parameters[4] = new LibDbParameter { ParameterNm = "@ID", DbType = DbType.Int64, Direction = ParameterDirection.Output, Value = 0 };
                parameters[5] = new LibDbParameter { ParameterNm = "@storagetbnm", DbType = DbType.String, Size = 35, Direction = ParameterDirection.Output, Value = string.Empty };
                parameters[6] = new LibDbParameter { ParameterNm = "@storagetbfieldnm", DbType = DbType.String, Size = 35, Direction = ParameterDirection.Output, Value = string.Empty };
                udb.Database.ExeStoredProcedure("p_AddTableStorageInfo", parameters);
            }
        }
    }
}
