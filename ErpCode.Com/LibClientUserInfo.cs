using System;
using System.Collections.Generic;
using System.Text;

namespace ErpCode.Com
{
    public class LibClientUserInfo
    {
        public string SessionId { get; set; }

        public string UserId { get; set; }
        public string UserNm { get; set; }
        public string IP { get; set; }
        public string ClientNm { get; set; }

        public string ClientId { get; set; }

        public int Language { get; set; }

        /// <summary>用于存储自定义表信息数据存储的所在实际表名</summary>
        public string U_TBNm { get; set; }

        /// <summary>用于存储自定义表字段信息数据存储的所在实际表名</summary>
        public string U_TBFieldNm { get; set; }
    }
}
