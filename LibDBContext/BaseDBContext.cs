using ErpModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace LibDBContext
{
    public class BaseDBContext:DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=.;Database=ErpCodeTestDB;User Id=sa;Password=152625;");
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            PropertyInfo[] properties = this.GetType().GetProperties();
            if (properties != null && properties.Length > 0)
            {
                Type ptype;
                LibModelCore o;
                foreach (PropertyInfo p in properties)
                {
                    if (p.PropertyType.Name.Contains("dbset", StringComparison.OrdinalIgnoreCase))
                    {
                        ptype = p.PropertyType.GenericTypeArguments[0];
                        if (typeof(IEntityConfigure).IsAssignableFrom(ptype))
                        {
                            o = Activator.CreateInstance(ptype) as LibModelCore;
                            o.OnModelBuilder(modelBuilder);
                        }
                    }
                }
            }
            base.OnModelCreating(modelBuilder);
        }
    }
}
