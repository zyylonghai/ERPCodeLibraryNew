using ErpModels.AppDataLog;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace LibDBContext
{
    public class DataLogDBContext : BaseDBContext
    {
        public virtual DbSet<DataLogsM> DataLogsM { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=.;Database=ErpCodeApp_Log;User Id=sa;Password=152625;");

        }
    }
}
