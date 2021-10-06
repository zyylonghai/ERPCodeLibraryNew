using Library.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace ErpCode.Com
{
    public class LibSearchCondition
    {
        public string DSID { get; set; }
        public string TableNm { get; set; }
        public string FieldNm { get; set; }

        public SmodalSymbol Symbol { get; set; }

        public string  valu1 { get; set; }
        public string valu2 { get; set; }
        public Smodallogic Logic { get; set; }
    }

    public enum SmodalSymbol
    {
        ///<summary>等于</summary>
        //[LibReSource("等于")]
        Equal = 1,
        /// <summary> 大于</summary>
        //[LibReSource("大于")]
        MoreThan = 2,
        /// <summary> 小于</summary>
        //[LibReSource("小于")]
        LessThan = 3,
        /// <summary> 包含</summary>
        //[LibReSource("包含")]
        Contains = 4,
        /// <summary>[a,b]之间</summary>
        //[LibReSource("[a,b]之间")]
        Between = 5,
        /// <summary>不等于</summary>
        //[LibReSource("不等于")]
        NoEqual = 6,
        /// <summary>大于等于</summary>
        //[LibReSource("大于等于")]
        MoreAndEqual = 7,
        /// <summary>小于等于</summary>
        //[LibReSource("小于等于")]
        LessAndEqual = 8

    }

    public enum Smodallogic
    {
        ///<summary>And</summary>
        //[LibReSource("And")]
        And = 1,
        /// <summary> Or</summary>
        //[LibReSource("Or")]
        Or = 2,
    }
}
