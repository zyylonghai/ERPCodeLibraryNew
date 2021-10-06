using Library.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text;

namespace ErpCode.Com
{
    public class LibDataLock : LibLock
    {

        private object _data;
        #region 公共属性
        public string PrimaryValues { get; set; }
        public string ClientId { get; set; }
        #endregion
        #region  构造函数
        public LibDataLock()
        {
            
        }
        public LibDataLock(string clientid,object data)
        {
            this.ClientId = clientid;
            if (data != null)
            {
                this.Key = data.GetType().Name;
                this._data = data;
            }
            //this.PrimaryValues = GetPrimaryKeyvalue(data);
        }
        #endregion

        public bool HasExist(object data)
        {
            string key = GetPrimaryKeyvalue(data);
            return key == this.PrimaryValues;
        }

        #region 私有函数
        private string GetPrimaryKeyvalue(object data)
        {
            if (data == null) return  string.Empty;
            StringBuilder keystr = new StringBuilder();
            Type t = data.GetType();

            PropertyInfo[] ps = t.GetProperties();
            foreach (PropertyInfo p in ps)
            {
                if (p.GetCustomAttribute<KeyAttribute>() != null)
                {
                    if (keystr.Length > 0) {
                        keystr.Append(":");
                    }
                    keystr.Append(p.GetValue(data));
                }
            }
            return keystr.ToString();
        }
        #endregion 
        public override void Lock()
        {
            this.PrimaryValues = GetPrimaryKeyvalue(this._data);
            this.Status = LibLockStatus.Lock;
        }

        public override void UnLock()
        {
            this.PrimaryValues = null;
            this.Status = LibLockStatus.UnLock;
        }
    }
}
