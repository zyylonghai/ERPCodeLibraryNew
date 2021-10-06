using System;
using System.Collections.Generic;
using System.Text;

namespace ErpCode.Com
{
    public class LibClientDataInfo
    {
        public object Datas { get; set; }
        public object OldDatas { get; set; }
        public List<LibPrimaryKeyInfo> PrimaryKeyInfos { get; set; }
        public LibClientDataStatus clientDataStatus { get; set; }
    }
    public enum LibClientDataStatus
    {
        Add = 1,
        Edit = 2,
        Delete = 3,
        Preview=4
    }

    public class LibPrimaryKeyInfo
    {
        public string KeyColumn { get; set; }
        public object Value { get; set; }
    }
}
