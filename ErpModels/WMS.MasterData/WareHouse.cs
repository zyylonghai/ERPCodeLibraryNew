using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ErpModels.WMS.MasterData
{
    /// <summary>
    /// 仓库
    /// </summary>
    public class WareHouse:LibModelCore
    {
        [Key]
        public string WareHouseID { get; set; }
        public string WareHouseNm { get; set; }
        public string Remark { get; set; }

        public override void OnModelBuilder(ModelBuilder builder)
        {
            base.OnModelBuilder(builder);
            builder.Entity<WareHouse>().HasKey(i => new { i.WareHouseID });//设置主键
        }
    }
}
