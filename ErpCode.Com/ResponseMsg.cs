using System;
using System.Collections.Generic;
using System.Text;

namespace ErpCode.Com
{
    public class ResponseMsg
    {
        public bool IsSuccess { get; set; }
        public bool showMessageBox { get; set; }
        public string Message { get; set; }
        /// <summary>
        /// 1:错误信息。2：警告信息。3：提示信息
        /// </summary>
        public int MessageType { get; set; }
        /// <summary>
        /// 供开发者自行定义返回的数据
        /// </summary>
        public object Data { get; set; }
        /// <summary>
        /// 框架里的ClientDatas
        /// </summary>
        public List<LibClientDatas> ClientDatas { get; set; }

    }
}
