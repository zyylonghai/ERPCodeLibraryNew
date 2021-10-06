using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ErpModels.Appsys
{
    public class AuthorRecord : LibModelCore
    {
        /// <summary>终端IP</summary>
        [Key]
        public string IPAddress { get; set; }
        /// <summary>终端设备名</summary>
        public string FromAddress { get; set; }
        /// <summary>是否通过认证</summary>
        public bool AuthorPass { get; set; }

        public string remark { get; set; }

        public string other1 { get; set; }

    }
}
