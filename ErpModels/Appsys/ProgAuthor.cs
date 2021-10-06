using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace ErpModels.Appsys
{
    public class ProgAuthor : LibModelCore
    {
        public string TenantID { get; set; }
        public string ProgNm { get; set; }

        public override void OnModelBuilder(ModelBuilder builder)
        {
            base.OnModelBuilder(builder);
            builder.Entity<ProgAuthor>().HasKey(i => new { i.TenantID, i.ProgNm });
        }
    }
}
