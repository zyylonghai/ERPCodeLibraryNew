using ErpModels.Appsys;
using System;
using System.Collections.Generic;
using System.Text;

namespace ViewModels.AppViewModel
{
    public class ProgModels
    {
        public ProgInfo progInfo { get; set; }

        public List<ProgControlInfo> progControlInfos { get; set; }
        public List<ProgFieldInfo> progFieldInfos { get; set; }

        public RptHtmlInfo RptHtmlInfo { get; set; }
    }
}
