using ErpModels.WMS.MasterData;
using ErpModels.WMS.RawMaterial;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace LibDBContext.WMS
{
    public class WMSDBContext : BaseDBContext
    {
        public virtual DbSet<Materials> Materials { get; set; }
        public virtual DbSet <WareHouse> WareHouse { get; set; }
        public virtual DbSet<StorageArea> StorageArea { get; set; }
        public virtual DbSet<GoodSpace> GoodSpace { get; set; }
    }
}
