using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Library.Core.LibAttribute;
using Microsoft.EntityFrameworkCore;

namespace ErpModels.Author
{
    /// <summary>用户角色表</summary>
    public class UserRole:LibModelCore
    {
        [Key, Column(Order = 1)]
        public string AccountId { get; set; }

        [Key, Column(Order = 2)]
        [LibFromSource("Jole", "JoleId")]
        public string JoleId { get; set; }

        public override void OnModelBuilder(ModelBuilder builder)
        {
            base.OnModelBuilder(builder);
            builder.Entity<UserRole>().HasKey(i => new {i.AccountId ,i.JoleId });
        }
    }
}
