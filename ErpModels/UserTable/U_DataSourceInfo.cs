using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ErpModels.UserTable
{
    public  class U_DataSourceInfo : LibModelCore
    {
        //[Key, Column(Order = 1)]
        //public string ClientId { get; set; }

        [Key, Column(Order = 1)]
        public string DataSourceNm { get; set; }

        public string DataSourceDesc { get; set; }

        public override void OnModelBuilder(ModelBuilder builder)
        {
            base.OnModelBuilder(builder);
            builder.Entity<U_DataSourceInfo>().HasKey(i => new { i.ClientId , i.DataSourceNm });//设置主键
        }
    }
}
