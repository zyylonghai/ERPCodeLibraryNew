using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ErpModels.Appsys
{
    public class License:LibModelCore 
    {

        public string PublicKey { get; set; }
        //[Key]
        //public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string AuthenData { get; set; }
        [NotMapped]
        public string Privatekey { get; set; }

        public override void OnModelBuilder(ModelBuilder builder)
        {
            base.OnModelBuilder(builder);
            builder.Entity<License>().HasKey(i => new { i.ClientId });//设置主键
        }
    }

    [NotMapped]
    public class TenantInfo: INotmapEntyity
    {
        public string Authenticator { get; set; }
        public string Password { get; set; }

        public int Validaccounts { get; set; }

        public string Secret { get; set; }

        public DateTime ExpirationDT { get; set; }
    }
}
