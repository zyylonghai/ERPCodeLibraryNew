using ErpCode.Com.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ErpModels.UserTable
{
    public  class U_TableFieldInfo : LibModelCore
    {
        //[Key, Column(Order = 1)]
        //public string ClientId { get; set; }

        [Key, Column(Order = 1)]
        public string TableNm { get; set; }

        [Key, Column(Order = 2)]
        public string FieldNm { get; set; }

        public string FieldDesc { get; set; }

        public LibDataType DataType { get; set; }

        public bool IsPrimaryKey { get; set; }

        public int DataLength { get; set; }

        public int PointLength { get; set; }

        public override void OnModelBuilder(ModelBuilder builder)
        {
            base.OnModelBuilder(builder);
            builder.Entity<U_TableFieldInfo>().HasKey(i => new { i.ClientId, i.TableNm,i.FieldNm});//设置主键
        }
    }
}
