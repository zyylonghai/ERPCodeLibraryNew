using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ErpCode.Com
{
    public class LibJsonTool
    {
        public static TModel ClientDataToModel<TModel>(LibClientDataInfo clientDataInfo)
        {
            TModel result = Activator.CreateInstance<TModel>();
            PropertyInfo[] properties = typeof(TModel).GetProperties();
            var jobj = ((JObject)clientDataInfo.Datas).Children();
            foreach (JProperty item in jobj)
            {
                PropertyInfo p = properties.FirstOrDefault(i => i.Name == item.Name);
                if (p != null)
                {
                    p.SetValue(result, item.Value.ToObject(p.PropertyType), null);
                }
            }
            return result;
        }

        //public static List<TModel> ClientDataToModelList<TModel>()
    }
}
