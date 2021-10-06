using System;
using System.Collections.Generic;
using System.Text;

namespace ErpCode.Com
{
    public enum LibOperatAction
    {
        None = 0,
        /// <summary>
        /// 功能处于新增状态
        /// </summary>
        Add = 1,
        /// <summary>
        /// 功能处于编辑状态
        /// </summary>
        Edit = 2,
        ///// <summary>
        ///// 功能处于搜索后预览状态
        ///// </summary>
        //SearchView = 3,
        /// <summary>
        /// 功能处于保存后预览状态
        /// </summary>
        Preview = 4
    }
}
