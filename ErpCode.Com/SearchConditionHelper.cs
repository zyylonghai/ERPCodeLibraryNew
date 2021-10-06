using Library.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace ErpCode.Com
{
   public class SearchConditionHelper
    {
        public static void AnalyzeSearchCondition(List<LibSearchCondition> conds, StringBuilder whereformat, ref object[] values)
        {
            int n = 0;
            LibSearchCondition precond = null;
            int len = 0;
            foreach (LibSearchCondition item in conds)
            {
                if (whereformat.Length > 0)
                {
                    if (precond != null)
                        whereformat.AppendFormat(" {0} ", precond.Logic.ToString());
                }
                switch (item.Symbol)
                {
                    case SmodalSymbol.Equal:
                        whereformat.Append("" + item.FieldNm + "={" + n + "}");
                        len = 1;
                        break;
                    case SmodalSymbol.MoreThan:
                        whereformat.Append("" + item.FieldNm + ">{" + n + "}");
                        len = 1;
                        break;
                    case SmodalSymbol.LessThan:
                        whereformat.Append("" + item.FieldNm + "<{" + n + "}");
                        len = 1;
                        break;
                    case SmodalSymbol.Contains:
                        whereformat.Append("" + item.FieldNm + " like {" + n + "}");
                        item.valu1 = string.Format("%{0}%", item.valu1);
                        len = 1;
                        break;
                    case SmodalSymbol.Between:
                        whereformat.Append("" + item.FieldNm + " between {" + n + "} and {" + (n = n + 1) + "}");
                        len = 2;
                        break;
                    case SmodalSymbol.NoEqual:
                        whereformat.Append("" + item.FieldNm + "!={" + n + "}");
                        len = 1;
                        break;
                    case SmodalSymbol.LessAndEqual:
                        whereformat.Append("" + item.FieldNm + "<={" + n + "}");
                        len = 1;
                        break;
                    case SmodalSymbol.MoreAndEqual:
                        whereformat.Append("" + item.FieldNm + ">={" + n + "}");
                        len = 1;
                        break;
                }
                n++;
                for (int i = 0; i < len; i++)
                {
                    Array.Resize(ref values, values.Length + 1);
                    values[values.Length - 1] = i==0?item.valu1 : item.valu2;
                }
                precond = item;
            }
        }

        //public static List<string> AnalyzeSearchConditionforUData(List<LibSearchCondition> conds, string custbnm, string clientid)
        //{
            
        //}
    }

    public class WhereObject
    {
        protected string _whereformat = string.Empty;
        private string[] _params;
        protected string patter = @"{\w*}+";
        private List<string> _appendwhereformats = null;
        #region 公开属性
        public string WhereFormat
        {
            get
            {
                string result = _whereformat;
                MatchCollection matchs = Regex.Matches(result, patter);
                int index = 0;
                if (matchs.Count > 0)
                    _params = new string[matchs.Count];
                foreach (Match item in matchs)
                {
                    index = Convert.ToInt32(item.Value.Replace("{", "").Replace("}", ""));
                    _params[index] = string.Format("@V{0}", index);
                    result = result.Replace(item.Value, _params[index]);
                }
                return result;
            }
            set
            {
                _whereformat = value;
            }
        }

        public object[] Values
        {
            get;
            set;
        }

        public string ValueTostring
        {
            get
            {
                if (_params != null && _params.Length > 0)
                {
                    StringBuilder partype = new StringBuilder();
                    StringBuilder val = new StringBuilder();
                    object o = null;
                    Type t = null;
                    for (int n = 0; n < _params.Length; n++)
                    {
                        if (_params[n] == null) continue;
                        if (partype.Length > 0)
                        {
                            partype.Append(",");
                            val.Append(",");
                        }
                        o = Values[Convert.ToInt32(_params[n].Substring(2))];
                        t = o.GetType();
                        if (t == typeof(string))
                        {
                            partype.AppendFormat("{0} nvarchar({1})", _params[n], o.ToString().Length == 0 ? 1 : o.ToString().Length);
                            val.AppendFormat("{0}='{1}'", _params[n], o);
                        }
                        else if (t == typeof(int))
                        {
                            partype.AppendFormat("{0} int ", _params[n]);
                            val.AppendFormat("{0}={1}", _params[n], o);
                        }
                        else if (t == typeof(bool))
                        {
                            partype.AppendFormat("{0} bit ", _params[n]);
                            val.AppendFormat("{0}={1}", _params[n], o);
                        }
                        else if (t.BaseType.Equals(typeof(Enum)))
                        {
                            partype.AppendFormat("{0} int ", _params[n]);
                            val.AppendFormat("{0}={1}", _params[n], (int)o);
                        }
                    }
                    return string.Format("N'{0}',{1}", partype.ToString(), val.ToString());
                }
                else
                    return string.Empty;
            }
        }
        #endregion

        #region 公开方法
        public void AppendWhereFormat(string andor, string whereformat, params object[] values)
        {
            if (string.IsNullOrEmpty(this._whereformat))
                this._whereformat = whereformat;
            else
            {
                MatchCollection matchs = Regex.Matches(_whereformat, patter);
                if (matchs != null)
                {
                    int oldcout = matchs.Count;
                    matchs = Regex.Matches(whereformat, patter);
                    int index = 0;
                    foreach (Match item in matchs)
                    {
                        index = Convert.ToInt32(item.Value.Replace("{", "").Replace("}", ""));
                        whereformat = whereformat.Replace(item.Value, item.Value.Replace(index.ToString(), string.Format("{0}*", (oldcout + index).ToString())));
                    }
                }
                whereformat = whereformat.Replace("*", "");
                this._whereformat = string.Format("{0} {1} {2}", this._whereformat, andor, whereformat);
                if (_appendwhereformats == null) _appendwhereformats = new List<string>();
                _appendwhereformats.Add(string.Format("{0}", whereformat));
            }
            if (values != null)
            {
                List<object> vals = new List<object>();
                if (this.Values != null && this.Values.Length > 0)
                    vals.AddRange(this.Values);
                vals.AddRange(values);
                this.Values = vals.ToArray();
            }
        }
        #endregion
        #region 受保护的方法

        #endregion 

    }
}
