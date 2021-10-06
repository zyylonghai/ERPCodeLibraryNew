using System;
using System.Collections.Generic;
using System.Text;

namespace LibAnalyzeScript
{
    public class ElseInfo
    {
        public int BeginIndex { get; set; }
        public int EndIndex { get; set; }
        /// <summary>是否需要执行else 里面的代码</summary>
        public bool NeedExeCode { get; set; }
    }
}
