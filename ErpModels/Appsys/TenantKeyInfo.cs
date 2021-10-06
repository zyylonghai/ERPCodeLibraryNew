using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ErpModels.Appsys
{
    public class TenantKeyInfo : LibModelCore
    {
        //[Key]
        //public string ClientId { get; set; }
        public string Privatekey { get; set; }
        public string PublicKey { get; set; }

        public override void OnModelBuilder(ModelBuilder builder)
        {
            base.OnModelBuilder(builder);
            builder.Entity<TenantKeyInfo>().HasKey(i => new { i.ClientId });//设置主键
        }
    }
}
