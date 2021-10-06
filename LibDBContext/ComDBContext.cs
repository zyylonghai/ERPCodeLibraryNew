using ErpModels.Com;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace LibDBContext
{
    public class ComDBContext : BaseDBContext
    {
        public virtual DbSet<CheckBillMap> CheckBillMap { get; set; }
        public virtual DbSet<CheckBill> CheckBill { get; set; }
        //public virtual DbSet <Materials> Materials { get; set; }

        public virtual DbSet <CodeRule> CodeRule { get; set; }
        public virtual DbSet<CodeRuleD> CodeRuleD { get; set; }
        public virtual DbSet<CodeRuleConfig> CodeRuleConfig { get; set; }
        public virtual DbSet<UserMenu> UserMenu { get; set; }
    }
}
