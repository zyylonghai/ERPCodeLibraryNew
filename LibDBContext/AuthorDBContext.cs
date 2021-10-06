using ErpModels.Author;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace LibDBContext
{
    public class AuthorDBContext : BaseDBContext
    {
        public virtual DbSet<Account> Account { get; set; }
        public virtual DbSet <Jole> Jole { get; set; }
        public virtual DbSet <JoleD> JoleD { get; set; }

        public virtual DbSet<AuthorityObj> AuthorityObj { get; set; }

        public virtual DbSet <UserRole> UserRole { get; set; }
    }
}
