using ErpCode.Com.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ErpModels.Author
{
    public class Account : LibModelCore
    {
        [Key]
        public string AccountId { get; set; }

        public string AccountNm { get; set; }
        public string AccountDesc { get; set; }
        public string Password { get; set; }
        public LibAccountStatus Status { get; set; }

        public string loginIP { get; set; }
        public DateTime? LoginDT { get; set; }

        public bool IsLogin { get; set; }
        public string PasswordKey { get; set; }
    }


}
