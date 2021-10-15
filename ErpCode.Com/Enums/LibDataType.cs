using System;
using System.Collections.Generic;
using System.Text;

namespace ErpCode.Com.Enums
{
    public enum LibDataType
    {
        /// <summary>字符串</summary>
        String=1,
        /// <summary>日期时间</summary>
        DateTime = 2,
        /// <summary>日期</summary>
        Date = 3,
        /// <summary>数字</summary>
        Decimal =4,
        /// <summary>下拉选项</summary>
        Enums = 5,
        /// <summary>
        /// 搜索框
        /// </summary>
        Search = 6
    }
    /// <summary>表种类 </summary>
    public enum LibTableKind
    {
        /// <summary>
        /// 虚拟表
        /// </summary>
        Virtual=0,
        /// <summary>
        /// 实体表
        /// </summary>
        Entity=1
    }
}
