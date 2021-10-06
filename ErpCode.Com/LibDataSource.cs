using System;
using System.Collections.Generic;
using System.Text;

namespace ErpCode.Com
{
    public class LibDataSource
    {
        public string dsNm { get; set; }

        public string dsDesc { get; set; }
    }

    public class LibTableInfo
    {
        public string tableNm { get; set; }
        public string tableDesc { get; set; }
    }
    public class LibFieldInfo
    {
        public string fieldNm { get; set; }
        public string fieldDesc { get; set; }
        public LibFieldType fieldType { get; set; }
        /// <summary>是否来自LibModelCore的字段</summary>
        public bool isappfield { get; set; }

        /// <summary>是否不作为搜索条件</summary>
        public bool disabled { get; set; }
        /// <summary>该字段的表格栏位是否要隐藏</summary>
        public bool hidden { get; set; }
    }

    public enum LibFieldType
    {
        String=1,
        Datetime=2,
        Interger=3,
        Decimal=4,
        Enums=5,
        Search=6
           
    }
}
