using ErpCode.Com;
using ErpModels.Appsys;
using ErpModels.UserTable;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using ViewModels.AppViewModel;

namespace ErpCode.BaseApiController
{
    public class LibSessionData
    {
        //public LibLanguage Language { get; set; }
        //public List<string> TableNms { get; set; }
        public ProgModels ProgInfoData { get; set; }

        public LibOperatAction OperatAction { get; set; }

        public List<U_TableFieldInfo> u_TableFieldInfos { get; set; }
        ///// <summary>用于存储自定义表信息数据存储的所在实际表名</summary>
        //public  string U_TBNm { get; set; }

        ///// <summary>用于存储自定义表字段信息数据存储的所在实际表名</summary>
        //public string U_TBFieldNm { get; set; }

        public Dictionary<string, string> dataextjson=null;
        public LibSessionData()
        {
            //ProgInfoData = new ProgModels();
            //TableNms = new List<string>();
            dataextjson = new Dictionary<string, string>();
            //UserInfo = new LibClientUserInfo();
        }
        //public LibSessionDataExt DataExt { get; set; }
        public void AddDataExt<T>(string key, T obj)
        {
            //string key = typeof(T).Name;
            if (dataextjson.ContainsKey(key))
            {
                dataextjson.Remove(key);
            }
            dataextjson.Add(key, JsonConvert.SerializeObject(obj));
        }

        public void RemoveDataExt(string key)
        {
            if (dataextjson.ContainsKey(key))
            {
                dataextjson.Remove(key);
            }
        }
        public T GetDataExt<T>(string key)
        {
            //string key = typeof(T).Name;
            if (!dataextjson.ContainsKey(key))
            {
                return default(T);
            }
           return  JsonConvert.DeserializeObject<T>(dataextjson[key]);
        }
    }
}
