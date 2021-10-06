using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ErpModels.UserTable
{
    public class U_TableStorageInfo : LibModelCore
    {
        [Key]
        public long ID { get; set; }
        public string StorageTableNm { get; set; }

        public string StorageTableFieldNm { get; set; }
    }
}
