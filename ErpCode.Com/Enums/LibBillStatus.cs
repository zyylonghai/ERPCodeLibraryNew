using System;
using System.Collections.Generic;
using System.Text;

namespace ErpCode.Com.Enums
{
    public enum LibBillStatus
    {
        Open = 1,
        Close = 2
    }

    public enum LibAccountStatus
    {
        /// <summary>激活</summary>
        Activation = 1,
        /// <summary>冻结</summary>
        Frost = 2,
    }

    public enum LibStatus
    {
        /// <summary>可用</summary>
        Enable=1,
        /// <summary>不可用</summary>
        Unenable=2
    }
}
