using ErpCode.AppDal;
using ErpCode.BaseApiController;
using ErpCode.Com;
using ErpCode.Com.Enums;
using ErpModels;
using ErpModels.Appsys;
using ErpRptModels;
using LibDBContext;
using Library.Core;
using Library.Core.LibAttribute;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ErpCodeLibrary.ApiControllers.AppSys
{
    [Route("AppSys/[action]")]
    [ApiController]
    //[Authorize]
    public class AppSysController : DataBaseController<AppsysDal>
    {
        private readonly IWebHostEnvironment environment;
        public AppSysController(IWebHostEnvironment webHostEnvironment)
        {
            this.environment = webHostEnvironment;
        }
        public List<LibDataSource> GetAllDataSource()
        {
            List<LibDataSource> result = new List<LibDataSource>();
            if (IsDeveloper())//本框架开发者租户
            {
                var dslist = Assembly.Load(new AssemblyName("LibDBContext")).GetTypes()
                    .Where(type => !string.IsNullOrWhiteSpace(type.Namespace))
                    .Where(type => type.GetTypeInfo().IsClass)
                    .Where(type => type.GetTypeInfo().BaseType != null)
                    .Where(type => typeof(BaseDBContext).IsAssignableFrom(type) && type != typeof(BaseDBContext) && type != typeof(DataLogDBContext))
                    .ToList();
                foreach (Type t in dslist)
                {
                    result.Add(new LibDataSource { dsNm = t.Name, dsDesc = t.Name });
                }
            }
            else//用户租户
            {
                return this.tDal.GetUserDataSource();
            }
            return result;
        }
        public List<LibTableInfo> GetTableinfobyds(string dsnm)
        {
            List<LibTableInfo> result = new List<LibTableInfo>();
            if (IsDeveloper())
            {
                var ds = getDataSourceType(dsnm);
                if (ds != null)
                {
                    PropertyInfo[] properties = ds.GetProperties();
                    if (properties != null && properties.Length > 0)
                    {
                        Type ptype;
                        foreach (PropertyInfo p in properties)
                        {
                            if (p.PropertyType.Name.Contains("dbset", StringComparison.OrdinalIgnoreCase))
                            {
                                ptype = p.PropertyType.GenericTypeArguments[0];
                                if (typeof(IEntityConfigure).IsAssignableFrom(ptype) || typeof(INotmapEntyity).IsAssignableFrom(ptype))
                                {
                                    result.Add(new LibTableInfo { tableNm = ptype.Name, tableDesc = ptype.Name });
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                return this.tDal.GetUserTableInfos(dsnm);
            }
            return result;
        }

        public List<LibFieldInfo> GetFieldInfo(string dsnm, string tbnm)
        {
            List<LibFieldInfo> result = new List<LibFieldInfo>();
            if (IsDeveloper())
            {
                var ds = getDataSourceType(dsnm);
                if (ds != null)
                {
                    PropertyInfo[] properties = ds.GetProperties();
                    if (properties != null && properties.Length > 0)
                    {
                        Type ptype = null;
                        foreach (PropertyInfo p in properties)
                        {
                            if (p.PropertyType.Name.Contains("dbset", StringComparison.OrdinalIgnoreCase))
                            {
                                ptype = p.PropertyType.GenericTypeArguments[0];
                                if (typeof(IEntityConfigure).IsAssignableFrom(ptype))
                                {
                                    if (ptype.Name == tbnm)
                                        break;
                                }
                            }
                        }
                        if (ptype != null && ptype.Name == tbnm)
                        {
                            LibFieldInfo f = null;
                            properties = ptype.GetProperties();
                            if (properties != null && properties.Length > 0)
                            {
                                foreach (PropertyInfo p in properties)
                                {
                                    if (p.DeclaringType.Equals(typeof(LibModelCore)) &&
                                        (p.Name == AppConstManage.applogid || p.Name == "LibModelStatus" || p.Name == "LAY_CHECKED")) continue;
                                    f = new LibFieldInfo();
                                    f.fieldNm = p.Name;
                                    f.fieldDesc = p.Name;
                                    if (p.DeclaringType.Equals(typeof(LibModelCore)))
                                    {
                                        f.isappfield = true;
                                    }
                                    if (p.PropertyType.Equals(typeof(string)))
                                        f.fieldType = LibFieldType.String;
                                    else if (p.PropertyType.Equals(typeof(DateTime)))
                                        f.fieldType = LibFieldType.Datetime;
                                    else if (p.PropertyType.Equals(typeof(int)))
                                        f.fieldType = LibFieldType.Interger;
                                    else if (p.PropertyType.Equals(typeof(decimal)))
                                        f.fieldType = LibFieldType.Decimal;
                                    else if (p.PropertyType.BaseType.Equals(typeof(Enum)))
                                    {
                                        f.fieldType = LibFieldType.Enums;
                                    }
                                    if (p.GetCustomAttribute<LibFromSourceAttribute>() != null)
                                    {
                                        f.fieldType = LibFieldType.Search;
                                    }
                                    result.Add(f);
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                return this.tDal.GetUserTableFieldinfo(tbnm);
            }
            return result;
        }

        public List<LibEnumkeyvalue> GetElemType()
        {
            var result = LibAppUtils.GetenumFields<LibElemType>();
            var gridtoolbtn = result.FirstOrDefault(i => i.key == -1);
            result.Remove(gridtoolbtn);
            result.ForEach(i => { i.value = AppGetFieldDesc(string.Empty, typeof(LibElemType).Name, i.value); });
            return result;
        }

        public List<LibEnumkeyvalue> GetFieldOptions(string dsnm, string tbnm, string fieldnm)
        {
            List<LibEnumkeyvalue> result = null;
            var ds = getDataSourceType(dsnm);
            if (ds != null)
            {
                PropertyInfo[] properties = ds.GetProperties();
                if (properties != null && properties.Length > 0)
                {
                    Type ptype = null;
                    foreach (PropertyInfo p in properties)
                    {
                        if (p.PropertyType.Name.Contains("dbset", StringComparison.OrdinalIgnoreCase))
                        {
                            ptype = p.PropertyType.GenericTypeArguments[0];
                            if (typeof(IEntityConfigure).IsAssignableFrom(ptype))
                            {
                                if (ptype.Name == tbnm)
                                    break;
                            }
                        }
                    }
                    if (ptype != null && ptype.Name == tbnm)
                    {
                        properties = ptype.GetProperties();
                        if (properties != null && properties.Length > 0)
                        {
                            foreach (PropertyInfo p in properties)
                            {
                                if (p.Name == fieldnm)
                                {
                                    if (p.PropertyType.Equals(typeof(bool)))
                                    {
                                        if (result == null) result = new List<LibEnumkeyvalue>();
                                        result.Add(new LibEnumkeyvalue { key = 0, value = AppGetFieldDesc(string.Empty, p.PropertyType.Name, "0") });
                                        result.Add(new LibEnumkeyvalue { key = 1, value = AppGetFieldDesc(string.Empty, p.PropertyType.Name, "1") });
                                    }
                                    else
                                    {
                                        result = LibAppUtils.GetenumFields(p.PropertyType);
                                        result.ForEach(i => { i.value = AppGetFieldDesc(string.Empty, p.PropertyType.Name, i.value); });
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return result;
        }

        public ResponseMsg GetFieldDesc(string prognm, string tbnm, string fieldnm)
        {
            this.ResponseMsg.Data = this.AppGetFieldDesc(prognm, tbnm, fieldnm);
            return this.ResponseMsg;
        }

        public string GeExceptionLog(int limit, int page)
        {
            LogHelp logHelp = new LogHelp(this.environment.WebRootPath + "\\");
            var loginfos = logHelp.GetLogInfos().OrderByDescending(i => i.DateTime).ToList();
            var data = loginfos.Skip(limit * (page - 1)).Take(limit).ToList();
            var result = new { code = 0, msg = "success", count = loginfos.Count, data = data };

            return JsonConvert.SerializeObject(result);
        }

        public ResponseMsg ReadLogfile(string filenm)
        {
            LogHelp logHelp = new LogHelp(this.environment.WebRootPath + "\\");
            string content = logHelp.ReadLogFile(filenm);
            return new ResponseMsg { IsSuccess = true, Data = content };
            //return Json(new { message = AppCom.LogHelp.ReadLogFile(filenm) }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ResponseMsg Deletelogfile(List<string> files)
        {
            LogHelp logHelp = new LogHelp(this.environment.WebRootPath + "\\");
            logHelp.DeleteLogFileBatch(files);
            return new ResponseMsg { IsSuccess = true };
        }

        [HttpGet]
        public ResponseMsg GetRptDataSource()
        {
            List<LibDataSource> result = new List<LibDataSource>();
            var dslist = Assembly.Load(new AssemblyName("LibDBContext")).GetTypes()
                .Where(type => !string.IsNullOrWhiteSpace(type.Namespace))
                .Where(type => type.GetTypeInfo().IsClass)
                .Where(type => type.GetTypeInfo().BaseType != null)
                .Where(type => (typeof(RptBaseDataSource).IsAssignableFrom(type)) &&
                       type != typeof(BaseDBContext) && type != typeof(AppDBContext) && type != typeof(DataLogDBContext) && type != typeof(RptBaseDataSource))
                .ToList();
            foreach (Type t in dslist)
            {
                result.Add(new LibDataSource { dsNm = t.Name, dsDesc = t.Name });
                result.Add(new LibDataSource { dsNm = string.Format("{0}Notmap", t.Name), dsDesc = string.Format("{0}Notmap", t.Name) });
            }
            return new ResponseMsg { IsSuccess = true, Data = result };
        }

        public ResponseMsg GetRptTableinfobyds(string dsnm)
        {
            List<LibTableInfo> result = new List<LibTableInfo>();
            var ds = getRptDataSourceType(dsnm);
            if (ds != null)
            {
                PropertyInfo[] properties = ds.GetProperties();
                if (properties != null && properties.Length > 0)
                {
                    Type ptype;
                    foreach (PropertyInfo p in properties)
                    {
                        ptype = p.PropertyType;
                        if (typeof(IEntityConfigure).IsAssignableFrom(ptype) || typeof(IRptEntityConfig).IsAssignableFrom(ptype))
                        {
                            result.Add(new LibTableInfo { tableNm = ptype.Name, tableDesc = ptype.Name });
                        }
                    }
                }
            }
            return new ResponseMsg { IsSuccess = true, Data = result };
        }

        public ResponseMsg GetRptFieldInfo(string dsnm, string tbnm)
        {
            List<LibFieldInfo> result = new List<LibFieldInfo>();
            var ds = getRptDataSourceType(dsnm);
            if (ds != null)
            {
                PropertyInfo[] properties = ds.GetProperties();
                if (properties != null && properties.Length > 0)
                {
                    Type ptype = null;
                    foreach (PropertyInfo p in properties)
                    {
                        //if (p.PropertyType.Name.Contains("dbset", StringComparison.OrdinalIgnoreCase))
                        //{
                        ptype = p.PropertyType;
                        if (typeof(IEntityConfigure).IsAssignableFrom(ptype) || typeof(IRptEntityConfig).IsAssignableFrom(ptype))
                        {
                            if (ptype.Name == tbnm)
                                break;
                        }
                        //}
                    }
                    if (ptype != null && ptype.Name == tbnm)
                    {
                        LibFieldInfo f = null;
                        properties = ptype.GetProperties();
                        if (properties != null && properties.Length > 0)
                        {
                            foreach (PropertyInfo p in properties)
                            {
                                if (p.DeclaringType.Equals(typeof(LibModelCore)) &&
                                    (p.Name == AppConstManage.applogid || p.Name == "LibModelStatus" || p.Name == "LAY_CHECKED")) continue;
                                f = new LibFieldInfo();
                                f.fieldNm = p.Name;
                                f.fieldDesc = p.Name;

                                result.Add(f);
                            }
                        }
                    }
                }
            }
            return new ResponseMsg { IsSuccess = true, Data = result };
        }

        /// <summary>租户注册</summary>
        /// <param name="info"></param>
        /// <returns></returns>
        [HttpPost]
        public ResponseMsg TenantRegist(TenantInfo info)
        {
            if (!IsDeveloper())
            {
                this.AddMessage("无权限");
                return ResponseMsg;
            }
            if (info != null)
            {
                LibAuthenData authenData = new LibAuthenData();
                authenData.AuthenticateID = Guid.NewGuid().ToString();
                authenData.Authenticator = info.Authenticator;
                authenData.ExpirationDT = info.ExpirationDT;
                authenData.Password = info.Password;
                authenData.Validaccounts = info.Validaccounts;

                string[] keys = RSAHelp.GenerateKeys();
                string datastr = JsonConvert.SerializeObject(authenData);
                License license = new License();
                license.ClientId = this.tDal.GenerateClientId();
                license.Privatekey = keys[0];
                license.PublicKey = keys[1];
                license.ClientSecret = info.Secret;
                license.AuthenData = RSAHelp.EncryptPrivekey(datastr, license.Privatekey);
                int result= this.tDal.TenantRegist(license);
                //string ss = RSAHelp.DecryptPublickey(license.AuthenData, license.PublicKey);

            }

            return ResponseMsg;
        }

        #region 私有函数
        private Type getDataSourceType(string dsnm)
        {
            var dslist = Assembly.Load(new AssemblyName("LibDBContext")).GetTypes()
            .Where(type => !string.IsNullOrWhiteSpace(type.Namespace))
            .Where(type => type.GetTypeInfo().IsClass)
            .Where(type => type.GetTypeInfo().BaseType != null)
            .Where(type => typeof(BaseDBContext).IsAssignableFrom(type) && type != typeof(BaseDBContext))
            .ToList();
            return dslist.FirstOrDefault(i => i.Name == dsnm);
        }
        private Type getRptDataSourceType(string dsnm)
        {
            var dslist = Assembly.Load(new AssemblyName("LibDBContext")).GetTypes()
            .Where(type => !string.IsNullOrWhiteSpace(type.Namespace))
            .Where(type => type.GetTypeInfo().IsClass)
            .Where(type => type.GetTypeInfo().BaseType != null)
            .Where(type => typeof(RptBaseDataSource).IsAssignableFrom(type) && type != typeof(RptBaseDataSource))
            .ToList();
            return dslist.FirstOrDefault(i => i.Name == dsnm);
        }
        #endregion
    }
}