using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ErpModels.WMS.RawMaterial
{
    public class Materials : LibModelCore
    {
        [Key]
        public string MatId { get; set; }
        public string MatNm { get; set; }
        public string shuxing1 { get; set; }
        public string Remark { get; set; }

        public override void OnModelBuilder(ModelBuilder builder)
        {
            base.OnModelBuilder(builder);
            builder.Entity<Materials>().HasKey(i => new { i.MatId });//设置主键
        }
    }
}
