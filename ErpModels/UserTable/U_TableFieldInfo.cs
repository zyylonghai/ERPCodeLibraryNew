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

        /// <summary>是否虚拟字段</summary>
        public bool IsVirtual { get; set; }

        public int DataLength { get; set; }
        /// <summary>小数点后位数</summary>
        public int PointLength { get; set; }

        /// <summary>字段最大长度</summary>
        public int MaxLength { get; set; }

        /// <summary>
        /// 来源数据源
        /// </summary>
        public string FromDataSource { get; set; }
        /// <summary>
        /// 来源字段
        /// </summary>
        public string FromFieldNm { get; set; }
        /// <summary>
        /// 来源描述字段
        /// </summary>
        public string FromFieldDescNm { get; set; }

        public override void OnModelBuilder(ModelBuilder builder)
        {
            base.OnModelBuilder(builder);
            builder.Entity<U_TableFieldInfo>().HasKey(i => new { i.ClientId, i.TableNm,i.FieldNm});//设置主键
        }
    }
}
