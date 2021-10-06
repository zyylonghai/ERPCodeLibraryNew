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
    /// 货位
    /// </summary>
    public class GoodSpace:LibModelCore
    {
        /// <summary>
        /// 货位编号
        /// </summary>
        [Key]
        public string GoodSpaceID { get; set; }

        [LibFromSource("StorageArea", "StorageAreaID")]
        public string StorageAreaID { get; set; }

        public override void OnModelBuilder(ModelBuilder builder)
        {
            base.OnModelBuilder(builder);
            builder.Entity<GoodSpace>().HasKey(i => new {i.GoodSpaceID });//设置主键
        }


    }
}
