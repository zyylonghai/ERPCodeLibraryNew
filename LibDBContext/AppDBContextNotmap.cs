using ErpModels.Appsys;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace LibDBContext
{
    public class AppDBContextNotmap:BaseDBContext
    {
        public virtual DbSet<TenantInfo> TenantInfo { get; set; }
    }
}
