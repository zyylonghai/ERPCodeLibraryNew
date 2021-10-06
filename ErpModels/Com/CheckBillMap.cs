using Library.Core.LibAttribute;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ErpModels.Com
{
    public class CheckBillMap : LibModelCore
    {
        public CheckBillMap()
        {

        }
        [Key, Column(Order = 1)]
        [LibFromSource("CheckBill", "BillNo")]
        public string BillNo { get; set; }

        [Key, Column(Order = 2)]
        [LibIdentity(1)]
        public int RowNo { get; set; }
        [LibFromSource("CheckBill", "Checker")]
        public string check1 { get; set; }
        //public string sdp_logid { get; set; }
        public string check2 { get; set; }
        public string check3 { get; set; }

        public override void OnModelBuilder(ModelBuilder builder)
        {
            base.OnModelBuilder(builder);
            builder.Entity<CheckBillMap>().HasKey(i => new { i.BillNo, i.RowNo });//设置主键
        }
    }
}
