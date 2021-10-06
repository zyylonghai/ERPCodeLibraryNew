using System;
using System.Collections.Generic;
using System.Text;

namespace LibAnalyzeScript.API
{
    public class ComApi
    {
        public void Declare(string variableNm, string type, string defaultvalue, Dictionary<string, VariableInfo> dic)
        {
            if (string.IsNullOrEmpty(variableNm))
            {
                throw new Exception("变量名，不能为空");
            }
            if (string.IsNullOrEmpty(type))
            {
                throw new Exception("变量类型，不能为空");
            }
            bool isnulldefautval = string.IsNullOrEmpty(defaultvalue);
            switch (type.Trim().ToLower())
            {
                case VariableType.Int:
                    if (!isnulldefautval)
                    {
                        int o = 0;
                        if (int.TryParse(defaultvalue, out o))
                        {
                            VariableInfo v = new VariableInfo();
                            v.Type = type;
                            v.Value = o;
                            dic.Add(variableNm, v);
                        }
                    }
                    break;
                case VariableType.Decimal:
                    if (!isnulldefautval)
                    {
                        decimal o = 0;
                        if (decimal.TryParse(defaultvalue, out o))
                        {
                            VariableInfo v = new VariableInfo();
                            v.Type = type;
                            v.Value = o;
                            dic.Add(variableNm, v);
                        }
                    }
                    break;
                case VariableType.String:
                    if (!isnulldefautval)
                    {
                        VariableInfo v = new VariableInfo();
                        v.Type = type;
                        v.Value = defaultvalue == "''" ? string.Empty : defaultvalue.Replace("'", "");
                        dic.Add(variableNm, v);
                    }
                    break;
                case VariableType.Bool:
                    if (!isnulldefautval)
                    {
                        VariableInfo v = new VariableInfo();
                        v.Type = type;
                        if (string.Compare(defaultvalue.Trim(), "false", true) == 0)
                        {
                            v.Value = false;
                        }
                        else if (string.Compare(defaultvalue.Trim(), "true", true) == 0)
                        {
                            v.Value = true;
                        }
                        else
                        {
                            throw new Exception("变量类型为bool，默认值只能为false或true");
                        }
                        //v.Value = defaultvalue.Trim().ToLower ()=="false" ?false  : true;
                        dic.Add(variableNm, v);
                    }
                    break;
            }
        }

        public void SetValue(string variableNm, string val, Dictionary<string, VariableInfo> dic)
        {
            VariableInfo v = null;
            if (dic.TryGetValue(variableNm, out v))
            {
                if (v.Type == "string")
                    v.Value = val == "''" ? string.Empty : val.Replace("'", "");
                else if (v.Type == "int")
                {
                    v.Value = int.Parse(val);
                }
            }
        }

        public void Print(string content, Dictionary<string, VariableInfo> dic)
        {
            Console.WriteLine(content);
        }
    }

    public class VariableType
    {
        public const string String = "string";
        public const string Int = "int";
        public const string Decimal = "decimal";
        public const string Bool = "bool";
    }
}
