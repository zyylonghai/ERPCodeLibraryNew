using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ErpModels.UserTable
{
    public  class U_TableInfo : LibModelCore
    {
        //[Key, Column(Order = 1)]
        //public string ClientId { get; set; }

        [Key, Column(Order = 1)]
        public string TableNm { get; set; }

        public string TableDesc { get; set; }

        public string DataSourceNm { get; set; }

        public string DataTBNm { get; set; }

        public override void OnModelBuilder(ModelBuilder builder)
        {
            base.OnModelBuilder(builder);
            builder.Entity<U_TableInfo>().HasKey(i => new { i.ClientId, i.TableNm });//设置主键
        }
    }
}
