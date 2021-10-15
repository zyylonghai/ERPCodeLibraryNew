using Library.Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace ErpCode.Com
{
    public class LibAppUtils
    {
        public static List<LibEnumkeyvalue> GetenumFields<Tenum>()
        {
            Type t = typeof(Tenum);
            //List<LibEnumkeyvalue> result = new List<LibEnumkeyvalue>();
            //Array array = Enum.GetValues(t);
            //foreach (Tenum item  in array)
            //{
            //    result.Add(new LibEnumkeyvalue { key = Convert.ToInt32(item), value = item.ToString() });
            //    //result .Add ()
            //}
            //return result;
            return GetenumFields(t);
        }
        public static List<LibEnumkeyvalue> GetenumFields(Type t)
        {
            List<LibEnumkeyvalue> result = new List<LibEnumkeyvalue>();
            Array array = Enum.GetValues(t);
            foreach (var item in array)
            {
                result.Add(new LibEnumkeyvalue { key = Convert.ToInt32(item), value = item.ToString() });
                //result .Add ()
            }
            return result;
        }

        public static TModel JobjectToType<TModel>(LibClientDataInfo clientDataInfo)
        {
            return (TModel)JobjectToType(clientDataInfo, typeof(TModel));
        }
        /// <summary>
        /// 将JObject或JsonElement 转为目标类型targtype的 实例对象
        /// </summary>
        /// <param name="clientDataInfo"></param>
        /// <param name="targtype"></param>
        /// <returns></returns>
        public static object JobjectToType(LibClientDataInfo clientDataInfo, Type targtype)
        {
            if (clientDataInfo == null || clientDataInfo.Datas == null)
            {
                throw new LibExceptionBase("参数clientDataInfo不能为空。");
            }
            if (clientDataInfo.Datas.GetType().Equals(typeof(JsonElement)))
            {
                clientDataInfo.Datas = JsonConvert.DeserializeObject(((JsonElement)clientDataInfo.Datas).ToString());
            }
            if (clientDataInfo.Datas.GetType().Equals(targtype))
            {
                return clientDataInfo.Datas;
            }
            if (!clientDataInfo.Datas.GetType().Equals(typeof(JObject)))
            {
                throw new LibExceptionBase("参数clientDataInfo.Datas 类型必须是JObject或JsonElement");
            }
            if (targtype == null)
            {
                throw new LibExceptionBase("参数targtype类型值不能为null");
            }

            return JobjectToType(clientDataInfo.Datas, targtype);
            //object o = Activator.CreateInstance(targtype);
            //PropertyInfo[] properties = targtype.GetProperties();
            //var jobj = ((JObject)clientDataInfo.Datas).Children();
            //foreach (JProperty item in jobj)
            //{
            //    PropertyInfo p = properties.FirstOrDefault(i => i.Name.ToLower() == item.Name.ToLower());
            //    if (item.Name == AppConstManage.applogid && IsNULLOrEmpty(item.Value.ToObject(typeof(string))))
            //    {
            //        continue;
            //    }
            //    if (p != null)
            //    {
            //        if (string.IsNullOrEmpty(item.Value.ToString()) && (p.PropertyType.Equals(typeof(int)) || p.PropertyType.Equals(typeof(decimal))))
            //        {
            //            continue;
            //        }
            //        if (p.PropertyType.BaseType.Equals(typeof(Enum)) && string.IsNullOrEmpty(item.Value.ToString()))
            //        {
            //            continue;
            //        }
            //        //if (p.PropertyType.Equals(typeof(bool)))
            //        //    { }
            //        p.SetValue(o, item.Value.ToObject(p.PropertyType), null);
            //    }
            //}
            //return o;

        }
        public static object JobjectToType(object target, Type targetype)
        {
            if (target == null)
                return null;
            if (targetype == null)
            {
                throw new LibExceptionBase("参数targtype类型值不能为null");
            }
            if (target.GetType().Equals(typeof(JsonElement)))
            {
                target = JsonConvert.DeserializeObject(((JsonElement)target).ToString());
            }
            if (!target.GetType().Equals(typeof(JObject)))
            {
                throw new LibExceptionBase("参数target 类型必须是JObject或JsonElement");
            }
            object o = Activator.CreateInstance(targetype);
            PropertyInfo[] properties = targetype.GetProperties();
            var jobj = ((JObject)target).Children();
            foreach (JProperty item in jobj)
            {
                PropertyInfo p = properties.FirstOrDefault(i => i.Name.ToLower() == item.Name.ToLower());
                if (item.Name == AppConstManage.applogid && IsNULLOrEmpty(item.Value.ToObject(typeof(string))))
                {
                    continue;
                }
                if (p != null)
                {
                    if (string.IsNullOrEmpty(item.Value.ToString()) && (p.PropertyType.Equals(typeof(int)) || p.PropertyType.Equals(typeof(decimal))))
                    {
                        continue;
                    }
                    if (p.PropertyType.BaseType.Equals(typeof(Enum)) && string.IsNullOrEmpty(item.Value.ToString()))
                    {
                        continue;
                    }
                    if (p.PropertyType.Equals(typeof(bool)))
                    {
                        string v = item.Value.ToString().Trim();
                        if (string.IsNullOrEmpty(v))
                        {
                            item.Value = false;
                        }
                        else
                        {
                            if (string.Compare(v, "false", true) == 0 || string.Compare(v, "0", true)==0)
                            {
                                item.Value = false;
                            }
                            else if (string.Compare(v, "true", true) == 0 || string.Compare(v, "1", true) == 0 || string.Compare(v, "on", true) == 0)
                            {
                                item.Value = true;
                            }
                        }
                        
                        //item.Value = !string.IsNullOrEmpty(item.Value.ToString());
                    }
                    p.SetValue(o, item.Value.ToObject(p.PropertyType), null);
                }
            }
            return o;

        }
        /// <summary>
        /// 将源LibClientDataInfo 赋值给目标LibClientDataInfo
        /// </summary>
        /// <param name="src"></param>
        /// <param name="target"></param>
        /// <param name="excludeNull">是否排除NULL值的字段,默认false</param>
        public static void MapToFieldValue(LibClientDataInfo src, LibClientDataInfo target, bool excludeNull = false)
        {
            if (target == null)
            {
                throw new Exception("目标对象不允许为Null");
            }
            Type tsrc = src.Datas.GetType();
            Type ttarget = target.Datas.GetType();
            object srcvalu = null;
            //var srcjobjs = ((JObject)src.Datas).Children();
            if (ttarget.Equals(typeof(JObject)))
            {
                var targetjobjs = ((JObject)target.Datas).Children();
                if (tsrc.Equals(typeof(JObject)))
                {
                    var srcjobjs = ((JObject)src.Datas).Children();
                    foreach (JProperty p in srcjobjs)
                    {
                        foreach (JProperty p2 in targetjobjs)
                        {
                            if (p.Name.ToLower() == p2.Name.ToLower())
                            {
                                p2.Value = p.Value;
                            }
                        }
                    }
                }
                else if (tsrc.Equals(typeof(JsonElement)))
                {

                }
                else
                {
                    PropertyInfo[] psrc = tsrc.GetProperties();
                    foreach (PropertyInfo ps in psrc)
                    {
                        srcvalu = ps.GetValue(src.Datas);
                        if (srcvalu == null && excludeNull) continue;
                        foreach (JProperty pt in targetjobjs)
                        {
                            if (ps.Name.ToLower() == pt.Name.ToLower())
                            {
                                pt.Value = (JToken)srcvalu;
                            }
                        }
                    }
                }
            }
            else
            {
                PropertyInfo[] properties = target.Datas.GetType().GetProperties();
                if (tsrc.Equals(typeof(JObject)))
                {
                    var srcjobjs = ((JObject)src.Datas).Children();
                    foreach (JProperty item in srcjobjs)
                    {
                        PropertyInfo p = properties.FirstOrDefault(i => i.Name.ToLower() == item.Name.ToLower());
                        if (p != null)
                        {
                            if (p.PropertyType.Equals(typeof(bool)))
                            {
                                p.SetValue(target.Datas, ConverToBool(item.Value), null);
                            }
                            else
                                p.SetValue(target.Datas, item.Value.ToObject(p.PropertyType), null);
                        }
                    }
                }
                else if (tsrc.Equals(typeof(JsonElement)))
                {
                    var srcjobjs = JsonConvert.DeserializeObject<JObject>(src.Datas.ToString()).Children();
                    foreach (JProperty item in srcjobjs)
                    {
                        PropertyInfo p = properties.FirstOrDefault(i => i.Name.ToLower() == item.Name.ToLower());
                        if (p != null)
                        {
                            if (p.PropertyType.Equals(typeof(bool)))
                            {
                                p.SetValue(target.Datas, ConverToBool(item.Value), null);
                            }
                            else
                                p.SetValue(target.Datas, item.Value.ToObject(p.PropertyType), null);
                        }
                    }
                }
                else
                {
                    PropertyInfo[] psrc = tsrc.GetProperties();
                    foreach (PropertyInfo ps in psrc)
                    {
                        srcvalu = ps.GetValue(src.Datas);
                        if (srcvalu == null && excludeNull) continue;
                        PropertyInfo p = properties.FirstOrDefault(i => i.Name.ToLower() == ps.Name.ToLower());
                        if (p != null)
                        {
                            p.SetValue(target.Datas, srcvalu, null);
                        }
                    }
                }
            }
        }

        //public static void 

        public static bool ConverToBool(object val)
        {
            if (string.Compare(val.ToString(), "False", true) == 0)
            {
                return false;
            }
            else if (string.Compare(val.ToString(), "True", true) == 0)
            {
                return true;
            }
            else if (string.Compare(val.ToString(), "on", true) == 0)
            {
                return true;
            }
            else if (string.Compare(val.ToString(), "1", true) == 0)
            {
                return true;
            }
            return false;
        }

        public static List<LibTreeObject> ConverToTreeObj<T>(List<T> data,LibTreeConfig config)
        {
            List<LibTreeObject> result = new List<LibTreeObject>();
            Type t = typeof(T);
            PropertyInfo[] ps = t.GetProperties();
            PropertyInfo idprop = ps.FirstOrDefault(i => i.Name == config.TreeIdCol);
            PropertyInfo pidprop = ps.FirstOrDefault(i => i.Name == config.PTreeIdCol);
            PropertyInfo titleprop = ps.FirstOrDefault(i => i.Name == config.TitleCol);
            PropertyInfo spreadprop = ps.FirstOrDefault(i => i.Name == config.PTreeIdCol);
            //List<T> roots=data.Where(i=>)
            long pid = -1;
            long id = -1;
            string title = string.Empty;
            bool spread = false;
            LibTreeObject treeObject = null;
            foreach (T item in data)
            {
                id = Convert.ToInt64(idprop.GetValue(item));
                pid = Convert.ToInt64(pidprop.GetValue(item));
                title = titleprop.GetValue(item).ToString();
                spread = ConverToBool(spreadprop.GetValue(item));
                treeObject = new LibTreeObject();
                treeObject.id = id;
                treeObject.title = title;
                treeObject.pid = pid;
                treeObject.spread = spread;
                result.Add(treeObject);
            }
            var list = result.Where(i => i.pid !=0).ToList();
            while (list != null && list.Count() > 0)
            {
                foreach (var item in list)
                {
                    var exit = result.FirstOrDefault(i => i.pid == item.id);
                    if (exit == null)
                    {
                        var ptr = result.FirstOrDefault(i => i.id == item.pid);
                        if (ptr != null)
                        {
                            Addtreeobj(ptr, item);
                            result.Remove(result.FirstOrDefault(i => i.id == item.id));
                        }

                    }
                }
                list = result.Where(i => i.pid != 0).ToList ();
            }
            return result;
        }

        private static  void Addtreeobj(LibTreeObject ptreeObject, LibTreeObject treeObject)
        {
            if (ptreeObject.children == null) ptreeObject.children = new List<LibTreeObject>();
            var v = ptreeObject.children.FirstOrDefault(i => i.id == treeObject.id);
            if (v == null)
            {
                LibTreeObject tree = new LibTreeObject();
                tree.id = treeObject.id;
                tree.title = treeObject.title;
                tree.pid = treeObject.pid;
                tree.children = treeObject.children;
                ptreeObject.children.Add(tree);
            }
        }

        public static bool IsNULLOrEmpty(object obj)
        {
            if (obj == null || obj == DBNull.Value)
            {
                return true;
            }
            else
            {
                if (string.Compare(obj.GetType().Name, typeof(string).Name, false) == 0 && obj.ToString() == string.Empty)
                {
                    return true;
                }
                else
                    return false;
            }
        }

        /// <summary>获取表达式中 括号内容 </summary>
        /// <returns></returns>
        public static string GetBracketContent(string express, BracketType bracketType)
        {
            Regex rgx = null;
            switch (bracketType)
            {
                case BracketType.Parentheses:
                    rgx= new Regex(@"(?i)(?<=\()(.*)(?=\))");//小括号()
                    break;
                case BracketType.Brackets:
                    rgx = new Regex(@"(?i)(?<=\[)(.*)(?=\])");//中括号[]
                    break;
                case BracketType.Big:
                    rgx = new Regex(@"(?i)(?<=\{)(.*)(?=\})");//大括号{}
                    break;
            }

            return rgx .Match (express).Value;
        }

        public static void CopyObject(object src, object target)
        {
            if (src == null)
            {
                target = null;
                return;
            }
            Type srctype = src.GetType();
            if (target == null)
            {
                target = Activator.CreateInstance(srctype);
                //throw new LibExceptionBase("目标对象不允许为null。");
            }
            Type targtype = target.GetType();
            if (!srctype.Equals(targtype))
            {
                //
                throw new LibExceptionBase("源对象与目标对象类型不一致。");
            }
            PropertyInfo[] targetps = targtype.GetProperties();
            PropertyInfo[] srcps = srctype.GetProperties();
            PropertyInfo srcp = null;
            foreach (var p in targetps)
            {
                srcp = srcps.FirstOrDefault(i => i.Name == p.Name);
                if (srcp != null)
                {
                    p.SetValue(target, srcp.GetValue(src));
                }
            }
        }
        public static object CopyFrom(object src)
        {
            object target=null;
            if (src == null)
                return target;
            Type srctype = src.GetType();
            target = Activator.CreateInstance(srctype);
            PropertyInfo[] ps = srctype.GetProperties();
            PropertyInfo[] tps = target.GetType().GetProperties();
            foreach (var p in ps)
            {
                var tp = tps.FirstOrDefault(i => i.Name == p.Name);
                if (tp != null)
                    tp.SetValue(target, p.GetValue(src));
            }
            return target;
        }

        public static Dictionary<string, object> CopyFrom(Dictionary<string, object> src)
        {
            Dictionary<string ,object> target = null;
            if (src == null)
                return target;
            target = new Dictionary<string, object>();
            foreach (var item in src)
            {
                target.Add(item.Key, item.Value);
            }
            return target;
        }

        public static void CopyDictions(Dictionary<string, object> src, Dictionary<string, object> target)
        {
            if (src == null) return;
            if (target == null) target = new Dictionary<string, object>();
            object v = null;
            foreach (var item in src)
            {
                if (target.TryGetValue(item.Key ,out v))
                {
                    target[item.Key] = item.Value;
                }
            }
        }


        //public static bool Compare(object a, object b)
        //{
        //    if ((a == null && b != null) || (b == null && a != null))
        //        return false;
        //    Type t = a.GetType();
        //    if (!t.Equals(b.GetType()))
        //        return false;
        //    if(a.Equals)
        //}
    }
    public class LibEnumkeyvalue
    {
        public int key { get; set; }
        public string value { get; set; }
    }

    public class LibTreeObject
    {
        public string title { get; set; }
        public long id { get; set; }
        public List<LibTreeObject> children { get; set; }

        public long  pid { get; set; }

        //public object otherValu { get; set; }
        /// <summary>
        /// 是否展开子节点
        /// </summary>
        public bool spread { get; set; }
    }

    public class LibTreeConfig
    {
        /// <summary> 节点ID 对应的列名</summary>
        public string TreeIdCol { get; set; }

        /// <summary>父节点ID，对应的列名</summary>
        public string PTreeIdCol { get; set; }

        /// <summary>节点名称，对应的列名</summary>
        public string TitleCol { get; set; }

        public string SpreadCol { get; set; }
    }

    public enum BracketType
    {
        /// <summary> 小括号</summary>
        Parentheses = 1,//
        /// <summary> 中括号</summary>
        Brackets = 2,//
        /// <summary>大括号</summary>
        Big = 3 //
    }
        
}
