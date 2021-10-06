using LibAnalyzeScript.API;
using Microsoft.ClearScript.V8;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace LibAnalyzeScript
{
    public class LibAnalyzeScriptManage
    {
        public static object ExecScriptCode(string scriptcode)
        {
            object result = null;
            IFInfo iFInfo = null, currentchild = null;
            ElseInfo elseInfo = new ElseInfo();
            int hasexecodelen = 0;
            //V8ScriptEngine engine = new V8ScriptEngine();
            //用于存储变量集合
            Dictionary<string, VariableInfo> _variabledic = new Dictionary<string, VariableInfo>();
            string[] funcs = scriptcode.Split(";");
            foreach (string item in funcs)
            {
                //hasexecodelen += item.Length;
                int index = -1, index2 = -1;
                string str = string.Empty, remainstr = string.Empty;
                index = item.IndexOf("(");
                if (index != -1)
                {
                    currentchild = GetCurrentChildInfo(iFInfo);
                    if (currentchild != null && !currentchild.ConditionResult && hasexecodelen >= currentchild.BeginIndex &&
                        hasexecodelen + item.Length <= currentchild.EndIndex)
                    {
                        hasexecodelen += item.Length + 1;
                        continue;
                    }
                    if (!elseInfo.NeedExeCode && hasexecodelen >= elseInfo.BeginIndex && hasexecodelen + item.Length <= elseInfo.EndIndex)
                    {
                        hasexecodelen += item.Length + 1;
                        continue;
                    }
                    str = item.Substring(0, index);
                    if (string.Compare(str.Trim(), "if", true) == 0)
                    {
                        //currentchild = GetCurrentChildInfo(iFInfo);
                        if (currentchild == null || currentchild.Root)
                        {
                            if (iFInfo == null || iFInfo.IsEnd) { iFInfo = new IFInfo { IsEnd = false, ConditionResult = false, Root = true }; }
                            currentchild = GetCurrentChildInfo(iFInfo);
                        }
                        if (hasexecodelen != 0 && hasexecodelen >= currentchild.BeginIndex &&
                            hasexecodelen + item.Length <= currentchild.EndIndex)
                        {
                            currentchild.Child = new IFInfo { IsEnd = false, ConditionResult = false, Root = false };
                            currentchild = currentchild.Child;
                        }
                        //currentchild = GetCurrentChildInfo(iFInfo);
                        //if (iFInfo.IsEnd) iFInfo = new IFInfo();
                        index2 = item.IndexOf("{");
                        remainstr = item.Substring(index, index2 - index);
                        if (!string.IsNullOrEmpty(remainstr))//此时remainstr 值为if 条件判断语句
                        {
                            //iFInfo = GetCurrentInfo(iFInfo);
                            //engine.Evaluate("function test(){" + string.Format("return {0};", remainstr) + "}");
                            //bool rt = DoIfConditionResult(remainstr);
                            //iFInfo.ConditionResult = rt;
                            currentchild.ConditionResult = DoIfConditionResult(remainstr, _variabledic);
                            if (currentchild != null)
                            {
                                currentchild.BeginIndex = hasexecodelen + index2;
                                currentchild.EndIndex = currentchild.BeginIndex + getEndBrackets(scriptcode.Substring(currentchild.BeginIndex));
                                //currentchild.IsEnd = !currentchild.ConditionResult;
                                //currentchild.ConditionResult = false;
                            }
                            if (currentchild.ConditionResult)
                            {
                                LogicParams parm = new LogicParams
                                {
                                    IFInfo = iFInfo,
                                    ElseInfo = elseInfo,
                                    hasexecodelen = hasexecodelen,
                                    index2 = index2,
                                    item = item,
                                    ScriptCode = scriptcode,
                                    Variabledic = _variabledic
                                };
                                ExcIfLogic(parm);
                                //parm.IFInfo = iFInfo;
                                //parm .
                                #region
                                //remainstr = item.Substring(index2 + 1);
                                //int index3 = -1, hasexecode2 = 0;
                                //string str2 = string.Empty;
                                //while (!string.IsNullOrEmpty(remainstr))
                                //{
                                //    index3 = remainstr.IndexOf("(");
                                //    if (index3 != -1)
                                //    {
                                //        str2 = remainstr.Substring(0, index3);
                                //        if (string.Compare(str2.Trim(), "if", true) == 0)
                                //        {
                                //            currentchild = GetCurrentChildInfo(iFInfo);
                                //            if (hasexecodelen != 0 &&
                                //                hasexecodelen + item.Length <= currentchild.EndIndex)
                                //            {
                                //                currentchild.Child = new IFInfo { IsEnd = false, ConditionResult = false, Root = false };
                                //            }
                                //            int index4 = remainstr.IndexOf("{");
                                //            str2 = remainstr.Substring(index3, index4 - index3);
                                //            if (!string.IsNullOrEmpty(str2))
                                //            {
                                //                if (currentchild.Child != null)
                                //                {
                                //                    currentchild.Child.ConditionResult = DoIfConditionResult(str2);
                                //                    currentchild.Child.BeginIndex = hasexecodelen + index2 + 1 + hasexecode2 + index4 + 1;
                                //                    currentchild.Child.EndIndex = currentchild.Child.BeginIndex + getEndBrackets(scriptcode.Substring(currentchild.Child.BeginIndex - 1));
                                //                    currentchild.Child.IsEnd = !currentchild.ConditionResult;
                                //                    //currentchild.ConditionResult = false;
                                //                }
                                //            }
                                //            remainstr = remainstr.Substring(index4 + 1);
                                //            hasexecode2 += index4;

                                //        }
                                //        else
                                //        {
                                //            remainstr = remainstr.Substring(remainstr.IndexOf(")") + 1);
                                //        }
                                //    }
                                //    else
                                //        remainstr = string.Empty;
                                //}
                                #endregion 
                            }
                            else
                            {
                            }
                        }
                    }
                    else if (str.Trim().StartsWith("}"))
                    {
                        int i = 0;
                        while (!string.IsNullOrEmpty(str) && str.Trim().StartsWith("}"))
                        {
                            currentchild = GetCurrentChildInfo(iFInfo);
                            i++;
                            if (currentchild != null && hasexecodelen + i >= currentchild.EndIndex)
                            {
                                currentchild.IsEnd = true;
                            }
                            str = str.Substring(1);
                        }
                        //currentchild = GetCurrentChildInfo(iFInfo);
                        //if (currentchild == null || (currentchild .ConditionResult&& hasexecodelen >= currentchild.BeginIndex && 
                        //    hasexecodelen + item.Length <= currentchild.EndIndex)||(!currentchild.ConditionResult&&!(hasexecodelen >= currentchild.BeginIndex &&
                        //    hasexecodelen + item.Length <= currentchild.EndIndex)))
                        //{
                        LogicParams parm = new LogicParams
                        {
                            IFInfo = iFInfo,
                            ElseInfo = elseInfo,
                            hasexecodelen = hasexecodelen,
                            index2 = item.IndexOf(str) - 1,
                            item = item,
                            ScriptCode = scriptcode,
                            Variabledic = _variabledic
                        };
                        ExcIfLogic(parm);
                        //}
                    }
                    else if (string.Compare(str.Trim(), "for", true) == 0)
                    {

                    }
                    else
                    {
                        index2 = item.IndexOf(str);
                        remainstr = item.Substring(index2 + str.Length + 1, item.Length - str.Length - 2);
                        string[] parmsstrarray = remainstr.Split(",");
                        List<object> parms = new List<object>();
                        parms.AddRange(parmsstrarray);
                        parms.Add(_variabledic);
                        ComApi comapi = new ComApi();
                        Type tp = typeof(ComApi);
                        MethodInfo meth = tp.GetMethod(str.Trim());
                        var p = meth.GetParameters();
                        object returnobj = meth.Invoke(comapi, parms.ToArray());
                        //meth .Invoke (comapi,)
                    }
                }
                else
                {
                    index = item.IndexOf("}");
                }
                hasexecodelen += item.Length + 1;
            }
            return result;
        }

        public static object ExecScriptCode2(string scriptcode)
        {
            Dictionary<string, VariableInfo> _variabledic = new Dictionary<string, VariableInfo>();
            object result = null;
            LogicParams p = new LogicParams();
            p.ScriptCode = scriptcode;
            p.Variabledic = _variabledic;
            AnalyScript(p);
            return result;
        }

        private static void SetIfInfoEnd(IFInfo info)
        {
            if (info != null)
            {
                if (info.Child != null)
                {
                    SetIfInfoEnd(info.Child);
                }
                else
                {
                    info.IsEnd = true;
                }
            }
        }

        private static IFInfo GetCurrentChildInfo(IFInfo info)
        {
            if (info == null)
                return null;
            if (info.Child != null && !info.Child.IsEnd)
                return GetCurrentChildInfo(info.Child);
            return info;
        }

        /// <summary>获取当前大括号对结束的下标值</summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private static int getEndBrackets(string str)
        {
            if (string.IsNullOrEmpty(str)) return 0;
            int index = str.IndexOf("}");
            int index2 = -1;
            string[] left, rigth;
            string s = str.Substring(0, index + 1);
            left = s.Split("{");
            rigth = s.Split("}");
            while (left.Length != rigth.Length)
            {
                s = str.Substring(index + 1);
                index2 = s.IndexOf("}");
                index += index2 + 1;
                s = str.Substring(0, index + 1);
                left = s.Split("{");
                rigth = s.Split("}");
            }
            return index;
        }

        /// <summary>判断if条件是否成立</summary>
        /// <param name="condition">if 条件</param>
        /// <returns></returns>
        private static bool DoIfConditionResult(string condition, Dictionary<string, VariableInfo> variableDic)
        {
            using (V8ScriptEngine engine = new V8ScriptEngine())
            {
                string[] array = condition.Split('(', ')', '=', '>', '<', '|', '&', '!');
                //for (int i = 1; i <= array.Length; i++)
                //{
                //    if (string.IsNullOrEmpty(array[i]))
                //    {

                //    }
                //}
                int index = 0;
                VariableInfo info = null;
                foreach (string item in array)
                {
                    if (string.IsNullOrEmpty(item))
                    {
                        index++;
                        continue;
                    }
                    if (variableDic.TryGetValue(item.Trim('[', ']'), out info))
                    {
                        switch (info.Type.Trim().ToLower())
                        {
                            case "string":
                                condition = condition.Replace(item, string.Format("'{0}'", info.Value));
                                break;
                            case "int":
                            case "decimal":
                                condition = condition.Replace(item, string.Format("{0}", info.Value));
                                break;
                            case "bool":
                                condition = condition.Replace(item, string.Format("{0}", (bool)info.Value ? "true" : "false"));
                                break;
                        }
                        //condition = condition.Replace(item, info.Value);
                    }
                }
                engine.Evaluate("function test(){" + string.Format("return {0};", condition) + "}");
                return (bool)engine.Script.test();
            }
        }

        private static void ExcIfLogic(LogicParams parms)
        {
            string remainstr = parms.item.Substring(parms.index2 + 1);
            int index3 = -1, hasexecode2 = 0;
            string str2 = string.Empty;
            IFInfo currentchild = null;
            while (!string.IsNullOrEmpty(remainstr))
            {
                index3 = remainstr.IndexOf("(");
                if (index3 != -1)
                {
                    str2 = remainstr.Substring(0, index3);
                    if (string.Compare(str2.Trim(), "if", true) == 0)
                    {
                        currentchild = GetCurrentChildInfo(parms.IFInfo);
                        if (parms.hasexecodelen != 0 &&
                            parms.hasexecodelen + parms.item.Length <= currentchild.EndIndex)
                        {
                            currentchild.Child = new IFInfo { IsEnd = false, ConditionResult = false, Root = false };
                        }
                        int index4 = remainstr.IndexOf("{");
                        str2 = remainstr.Substring(index3, index4 - index3);
                        if (!string.IsNullOrEmpty(str2))
                        {
                            bool conditionresult = DoIfConditionResult(str2, parms.Variabledic);
                            if (currentchild != null && (currentchild.IsEnd && currentchild.Root))
                            {
                                currentchild.ConditionResult = conditionresult;
                                currentchild.BeginIndex = parms.hasexecodelen + parms.index2 + 1 + hasexecode2 + index4;
                                currentchild.EndIndex = currentchild.BeginIndex + getEndBrackets(parms.ScriptCode.Substring(currentchild.BeginIndex));
                                currentchild.Child = null;
                                currentchild.IsEnd = false;
                            }
                            if (currentchild.Child != null)
                            {
                                currentchild.Child.ConditionResult = conditionresult;
                                currentchild.Child.BeginIndex = parms.hasexecodelen + parms.index2 + 1 + hasexecode2 + index4;
                                currentchild.Child.EndIndex = currentchild.Child.BeginIndex + getEndBrackets(parms.ScriptCode.Substring(currentchild.Child.BeginIndex));
                                //currentchild.Child.IsEnd = !currentchild.Child.ConditionResult;
                                //currentchild.ConditionResult = false;
                            }
                            if (!conditionresult)
                            {
                                if (!(remainstr.Contains("}", StringComparison.OrdinalIgnoreCase) && remainstr.Contains("else", StringComparison.OrdinalIgnoreCase)))
                                {
                                    break;
                                }
                            }
                        }
                        remainstr = remainstr.Substring(index4 + 1);
                        hasexecode2 += index4 + 1;

                    }
                    else if (str2.Trim().StartsWith("else", StringComparison.OrdinalIgnoreCase))
                    {
                        //parms.ElseInfo = new ElseInfo();
                        int index4 = remainstr.IndexOf("{");
                        parms.ElseInfo.BeginIndex = parms.hasexecodelen + parms.index2 + 1 + index4 + hasexecode2;
                        parms.ElseInfo.EndIndex = parms.ElseInfo.BeginIndex + getEndBrackets(parms.ScriptCode.Substring(parms.ElseInfo.BeginIndex));
                        currentchild = GetCurrentChildInfo(parms.IFInfo);
                        if (currentchild != null)
                        {
                            if (currentchild.EndIndex < parms.ElseInfo.BeginIndex)
                            {
                                parms.ElseInfo.NeedExeCode = currentchild.Root && !currentchild.ConditionResult;
                            }
                            else
                            {
                                parms.ElseInfo.NeedExeCode = (currentchild.Child != null && !currentchild.Child.ConditionResult);
                            }
                        }
                        else
                        {
                            parms.ElseInfo.NeedExeCode = false;
                        }
                        if (!parms.ElseInfo.NeedExeCode)
                        {
                            if (remainstr.Contains("}"))
                            {
                                remainstr = remainstr.Substring(remainstr.IndexOf("}") + 1);
                            }
                            else
                                remainstr = string.Empty;
                        }
                        else
                        {
                            remainstr = remainstr.Substring(remainstr.IndexOf("{") + 1);
                        }
                        //if (currentchild != null && ((currentchild.Root && !currentchild.ConditionResult) || (currentchild.Child != null && !currentchild.Child.ConditionResult)))
                        //{
                        //    parms.ElseInfo.NeedExeCode = true;
                        //    remainstr = remainstr.Substring(remainstr.IndexOf("{") + 1);
                        //}
                        //else
                        //{
                        //    parms.ElseInfo.NeedExeCode = false;
                        //    if (remainstr.Contains("}"))
                        //    {
                        //        remainstr = remainstr.Substring(remainstr.IndexOf("}") + 1);
                        //    }
                        //    else
                        //        remainstr = string.Empty;
                        //}
                        hasexecode2 += index4 + 1;
                    }
                    else if (str2.Trim().StartsWith("}", StringComparison.OrdinalIgnoreCase))
                    {
                        int i = 0;
                        int len = parms.item.IndexOf(str2);
                        while (!string.IsNullOrEmpty(str2) && str2.Trim().StartsWith("}"))
                        {
                            currentchild = GetCurrentChildInfo(parms.IFInfo);
                            i++;
                            if (currentchild != null && parms.hasexecodelen + len + i >= currentchild.EndIndex)
                            {
                                currentchild.IsEnd = true;
                            }
                            str2 = str2.Substring(1);
                        }
                        remainstr = remainstr.Substring(i);
                        hasexecode2 += i;
                    }
                    else if (str2.Trim().StartsWith("for", StringComparison.OrdinalIgnoreCase))
                    {

                    }
                    else
                    {
                        //currentchild = GetCurrentChildInfo(parms.IFInfo);
                        //if ((currentchild != null && currentchild.ConditionResult) || currentchild == null)
                        ExeFunc(new LogicParams { item = remainstr, Variabledic = parms.Variabledic });
                        remainstr = remainstr.Substring(remainstr.IndexOf(")") + 1);
                    }
                }
                else
                    remainstr = string.Empty;
            }
        }

        private static object ExeFunc(LogicParams parms)
        {
            int dex = parms.item.IndexOf("(");
            string str2 = parms.item.Substring(0, dex);
            int indx = parms.item.IndexOf(str2);
            string func = parms.item.Substring(indx + str2.Length + 1, parms.item.Length - str2.Length - 2);
            string[] parmsstrarray = func.Split(",");
            List<object> pms = new List<object>();
            pms.AddRange(parmsstrarray);
            pms.Add(parms.Variabledic);
            ComApi comapi = new ComApi();
            Type tp = typeof(ComApi);
            MethodInfo meth = tp.GetMethod(str2.Trim());
            var p = meth.GetParameters();
            object returnobj = meth.Invoke(comapi, pms.ToArray());
            return returnobj;
        }

        private static void ExeforLogic(LogicParams parms)
        {

        }

        private static void AnalyScript(LogicParams parms)
        {
            if (string.IsNullOrEmpty(parms.ScriptCode))
                return;
            string code = parms.ScriptCode;
            string funcode = string.Empty;
            string funNm = string.Empty;
            string elsefun = string.Empty;
            IFInfo iFInfo = null;
            LogicParams p = null;
            while (!string.IsNullOrEmpty(code))
            {
                int index = code.IndexOf(";");
                if (index > 0)
                {
                    funcode = code.Substring(0, index);
                    int leftkh = funcode.IndexOf("(");
                    funNm = funcode.Substring(0, leftkh);
                    int i = funcode.IndexOf("{");
                    if (i != -1)
                        elsefun = funcode.Substring(0, i);
                    else
                        elsefun = string.Empty;
                    if (!string.IsNullOrEmpty(funNm))
                    {
                        if (string.Compare(funNm.Trim(), "if", true) == 0)
                        {
                            iFInfo = new IFInfo();
                            iFInfo.ConditionResult = DoIfConditionResult(funcode.Substring(leftkh, i - leftkh), parms.Variabledic);
                            int endindex = getEndBrackets(code);
                            if (iFInfo.ConditionResult)
                            {
                                p = new LogicParams();
                                p.Variabledic = parms.Variabledic;
                                p.IFInfo = new IFInfo();
                                p.IFInfo.BeginIndex = i + 1;
                                p.IFInfo.EndIndex = endindex;
                                p.ScriptCode = code.Substring(p.IFInfo.BeginIndex, p.IFInfo.EndIndex - p.IFInfo.BeginIndex);
                                AnalyScript(p);
                            }
                            code = code.Substring(endindex + 1);
                        }
                        else if (string.Compare(elsefun.Trim(), "else", true) == 0)
                        {
                            int endindex = getEndBrackets(code);
                            if (iFInfo != null && !iFInfo.ConditionResult)
                            {
                                p = new LogicParams();
                                p.Variabledic = parms.Variabledic;
                                p.ElseInfo = new ElseInfo();
                                p.ElseInfo.BeginIndex = i + 1;
                                p.ElseInfo.EndIndex = endindex;
                                p.ScriptCode = code.Substring(p.ElseInfo.BeginIndex, p.ElseInfo.EndIndex - p.ElseInfo.BeginIndex);
                                AnalyScript(p);
                            }
                            code = code.Substring(endindex + 1);
                        }
                        else
                        {
                            ExeFunc(new LogicParams { item = funcode, Variabledic = parms.Variabledic });
                            code = code.Substring(index + 1);
                        }
                    }

                }
                else
                {
                    if (index == -1) break;
                    code = code.Substring(1);
                }
            }
        }

        //private static bool NeedSkipCode(int beginindex,int codelen,IFInfo info)
        //{

        //}
    }

    public class LogicParams
    {
        public IFInfo IFInfo { get; set; }
        public ElseInfo ElseInfo { get; set; }
        public Dictionary<string, VariableInfo> Variabledic { get; set; }

        public string ScriptCode { get; set; }

        public string item { get; set; }
        public int index2 { get; set; }
        public int hasexecodelen { get; set; }

    }
}
