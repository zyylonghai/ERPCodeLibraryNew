using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace ErpModels.Appsys
{
    public class ProgControlInfo:LibModelCore
    {
        /// <summary>
        /// 功能名（主键)
        /// </summary>
        [Key, Column(Order=1)]
        public string ProgNm { get; set; }

        [Key, Column(Order = 2)]
        public string ID { get; set; }

        public LibControlType ControlType { get; set; }

        public string Title { get; set; }
        public string DataSourceNm { get; set; }
        public string TableNm { get; set; }
        public int OrderNo { get; set; }
        public override void OnModelBuilder(ModelBuilder builder)
        {
            base.OnModelBuilder(builder);
            builder.Entity<ProgControlInfo>().HasKey(i => new { i.ProgNm , i.ID,i.ClientId });
        }

    }
    public enum LibControlType
    {
        Colla=1,
        Grid=2,
        Btns=3,
        RptTable=4,
        RptGrid=5
    }
    
}
