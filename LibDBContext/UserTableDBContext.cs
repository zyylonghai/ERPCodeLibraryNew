using ErpModels.UserTable;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace LibDBContext
{
    public class UserTableDBContext : BaseDBContext
    {
        public virtual DbSet<U_DataSourceInfo> U_DataSourceInfo { get; set; }

        public virtual DbSet<U_TableInfo> U_TableInfo { get; set; }
        public virtual DbSet<U_TableFieldInfo> U_TableFieldInfo { get; set; }

        public virtual DbSet<U_TableStorageInfo> U_TableStorageInfo { get; set; }
    }
}
