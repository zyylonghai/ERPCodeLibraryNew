using ErpModels.Appsys;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace LibDBContext
{
    public  class AppDBContext : BaseDBContext
    {
        public virtual DbSet<ProgInfo> ProgInfo { get; set; }
        public virtual DbSet<ProgControlInfo> ProgControlInfo { get; set; }
        public virtual DbSet<ProgFieldInfo> ProgFieldInfo { get; set; }
        public virtual DbSet<RptHtmlInfo> RptHtmlInfo { get; set; }

        public virtual DbSet<LanguageField> LanguageField { get; set; }

        public virtual DbSet <License> License { get; set; }
        public virtual DbSet<TenantKeyInfo> TenantKeyInfo { get; set; }

        public virtual DbSet<ProgAuthor> ProgAuthor { get; set; }

        public virtual DbSet<AuthorRecord> AuthorRecord { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=.;Database=ErpCodeApp;User Id=sa;Password=152625;");
            
        }

    }
}
