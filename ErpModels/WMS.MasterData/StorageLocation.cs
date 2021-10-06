using Library.Core.LibAttribute;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ErpModels.WMS.MasterData
{
    /// <summary>
    /// 库区
    /// </summary>
    public class StorageArea:LibModelCore
    {
        [Key]
        public string StorageAreaID { get; set; }

        [LibFromSource("WareHouse", "WareHouseID")]
        public string WareHouseID { get; set; }

        public string StorageAreaNm { get; set; }
        public string Remark { get; set; }

        public override void OnModelBuilder(ModelBuilder builder)
        {
            base.OnModelBuilder(builder);
            builder.Entity<StorageArea>().HasKey(i => new { i.StorageAreaID});//设置主键
        }
    }
}
