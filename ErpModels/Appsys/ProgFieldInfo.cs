using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using ErpCode.Com.Enums;
using Microsoft.EntityFrameworkCore;

namespace ErpModels.Appsys
{
    public class ProgFieldInfo:LibModelCore 
    {
        /// <summary>
        /// 功能名（主键)
        /// </summary>
        [Key, Column(Order = 1)]
        public string ProgNm { get; set; }
        /// <summary>
        /// 控件ID （来源ProgControlInfo）
        /// </summary>
        [Key, Column(Order = 2)]
        public string ID { get; set; }
        [Key, Column(Order = 3)]
        public string Field { get; set; }
        [NotMapped]
        public string Title { get; set; }
        public bool IsHide { get; set; }
        public bool OnlyRead { get; set; }
        public bool IsOnline { get; set; }
        /// <summary>
        /// 是否必填
        /// </summary>
        public bool IsRequired { get; set; }
        /// <summary>
        /// 是否只数字
        /// </summary>
        public bool IsNumber { get; set; }

        public LibElemType ElemType { get; set; }
        public string DefaultValue { get; set; }
        public int OrderNo { get; set; }
        public bool Onchange { get; set; }
        public bool Onclick { get; set; }
        public bool Onfocus { get; set; }

        public bool Onblur { get; set; }
        public bool OnKeydown { get; set; }

        /// <summary>点击事件触发的函数</summary>
        public string ClickFunc { get; set; }

        public int FieldLength { get; set; }
        public string RelateFieldExpress { get; set; }
        public string FromSourceTB { get; set; }
        public string ValidExpress { get; set; }

        public string RptColId { get; set; }

        public string Remark { get; set; }

        public override void OnModelBuilder(ModelBuilder builder)
        {
            base.OnModelBuilder(builder);
            builder.Entity<ProgFieldInfo>().HasKey(i => new { i.ProgNm, i.ID,i.Field ,i.ClientId});
        }
    }


}
