using System;
using System.Collections.Generic;
using System.Text;

namespace ErpCode.Com.Enums
{
    public enum LibElemType
    {
        /// <summary>单行文本框</summary>
        Singleline = 1,
        /// <summary>多行文本框</summary>
        Multiline = 2,
        /// <summary>下拉选项框</summary>
        DropdownOption = 3,
        /// <summary>日期</summary>
        Date = 4,
        /// <summary>日期时间</summary>
        DateTime = 5,
        /// <summary>复选框</summary>
        CheckBox = 6,
        /// <summary>单选框</summary>
        RadioBox = 7,
        /// <summary>图片</summary>
        Imgage = 8,
        /// <summary>密码框</summary>
        Password=9,
        /// <summary>搜索控件</summary>
        Search = 10,
        /// <summary>表格工具栏按钮</summary>
        GridToolBtn=-1,
    }

}
