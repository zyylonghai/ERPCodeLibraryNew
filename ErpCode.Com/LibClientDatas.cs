using Library.Core;
using Library.Core.LibAttribute;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace ErpCode.Com
{
    public class LibClientDatas
    {
        #region 私有属性
        #endregion 
        //public string ControlID { get; set; }
        public string DataSource { get; set; }
        public string TableNm { get; set; }
        public List<LibClientDataInfo> ClientDataInfos { get; set; }
        public List<string> collas { get; set; }
        public Dictionary<string, LibIdentityAttribute> _identitydic { get; set; }

        public LibClientDatas()
        {
            ClientDataInfos = new List<LibClientDataInfo>();
            collas = new List<string>();
            _identitydic = new Dictionary<string, LibIdentityAttribute>();
        }
        /// <summary>
        /// 自增长列集合
        /// </summary>
        /// <param name="modeltype"></param>
        public void CollectIdentity(Type modeltype)
        {
            if (modeltype == null) return;
            PropertyInfo[] ps = modeltype.GetProperties();
            LibIdentityAttribute identity = null;
            foreach (var p in ps)
            {
                if (!_identitydic.TryGetValue(string .Format("{0}", p.Name), out identity))
                {
                    identity = p.GetCustomAttribute<LibIdentityAttribute>();
                    if (identity != null)
                        _identitydic.Add(string.Format("{0}",p.Name), identity);
                }
            }
        }

        /// <summary>
        /// 对自增长字段，赋值。
        /// </summary>
        /// <param name="data">实体对象</param>
        public void SetIdentityValue(object data)
        {
            foreach (KeyValuePair<string ,LibIdentityAttribute> item in _identitydic)
            {
                data.GetType().GetProperty(item.Key).SetValue(data, item.Value.GetValue());
            }
        }

        public void SetTableIdentityCurrValue()
        {
            foreach (KeyValuePair<string, LibIdentityAttribute> item in _identitydic)
            {
                item.Value.SetCurrValue(GetTableIdentityValue(item.Key)); 
            }
        }

        private int GetTableIdentityValue(string identityfield)
        {
            int max = 0;
            int curr = 0;
            foreach (var item in this.ClientDataInfos)
            {
               curr =(int)item.Datas.GetType().GetProperty(identityfield).GetValue(item.Datas);
                if (curr > max)
                    max = curr;
            }
            return max;
        }
    }
}
