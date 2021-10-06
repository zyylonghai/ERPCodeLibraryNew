using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BaseApiController;
using ErpCode.Com;
using ErpModels;
using ErpModels.Appsys;
using LibDBContext;
using Microsoft.AspNetCore.Mvc;

namespace ErpCodeApi.Controllers.AppSys
{
    [Route("AppSys/[action]")]
    [ApiController]
    public class AppSysController : DataBaseController<AppDBContext>
    {

        public List<LibDataSource> GetAllDataSource()
        {
            List<LibDataSource> result = new List<LibDataSource>();
            var dslist = Assembly.Load(new AssemblyName("LibDBContext")).GetTypes()
                .Where(type => !string.IsNullOrWhiteSpace(type.Namespace))
                .Where(type => type.GetTypeInfo().IsClass)
                .Where(type => type.GetTypeInfo().BaseType != null)
                .Where(type => typeof(BaseDBContext).IsAssignableFrom(type) && type != typeof(BaseDBContext))
                .ToList();
            foreach (Type t in dslist)
            {
                result.Add(new LibDataSource { dsNm = t.Name, dsDesc = t.Name });
            }
            return result;
        }
        public List<LibTableInfo> GetTableinfobyds(string dsnm)
        {
            List<LibTableInfo> result = new List<LibTableInfo>();
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
                            if (typeof(IEntityConfigure).IsAssignableFrom(ptype))
                            {
                                result.Add(new LibTableInfo { tableNm = ptype.Name, tableDesc = ptype.Name });
                            }
                        }
                    }
                }
            }
            return result;
        }

        public List<LibFieldInfo> GetFieldInfo(string dsnm, string tbnm)
        {
            List<LibFieldInfo> result = new List<LibFieldInfo>();
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
                                result.Add(new LibFieldInfo { fieldNm = p.Name, fieldDesc = p.Name });
                            }
                        }
                    }
                }
            }
            return result;
        }

        public List<LibEnumkeyvalue> GetElemType()
        {
            return LibAppUtils.GetenumFields<LibElemType>();
        }

        //public ProgControlInfo Test()
        //{
        //    var data = (from a in dBContext.ProgInfo
        //                join b in dBContext.ProgControlInfo on a.ProgNm equals b.ProgNm
        //                select new { a, b }).ToList();
        //    if (data[0].b.ControlType == LibControlType.Colla)
        //    {

        //    }
        //    return data[0].b;
        //}

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

        #endregion
    }
}