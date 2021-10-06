using System;
using System.Collections.Generic;
using System.Text;

namespace LibAnalyzeScript
{
    public class IFInfo
    {
        public bool IsEnd { get; set; }
        public bool ConditionResult { get; set; }
        public bool Root { get; set; }
        public int BeginIndex { get; set; }
        public int EndIndex { get; set; }
        public IFInfo Child { get; set; }

    }
}
