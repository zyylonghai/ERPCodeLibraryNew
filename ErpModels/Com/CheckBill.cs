using ErpCode.Com.Enums;
using Library.Core.LibAttribute;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ErpModels.Com
{
    public class CheckBill : LibModelCore
    {
        [Key]
        public string BillNo { get; set; }
        public string Checker { get; set; }
        [LibFromSource("Materials", "MatId", "MatNm")]
        public string matId { get; set; }
        public decimal Qty { get; set; }
        public DateTime CheckDT { get; set; }
        public string CompanyId { get; set; }
        public string departmentId { get; set; }
        public string remark { get; set; }
        public LibBillStatus billStatus { get; set; }
        public byte[] img { get; set; }
        public string matid2 { get; set; }

        public decimal plantqty { get; set; }
    }
}
