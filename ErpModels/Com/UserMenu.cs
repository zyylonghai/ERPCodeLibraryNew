using Library.Core.LibAttribute;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ErpModels.Com
{
    public class UserMenu:LibModelCore
    {
        [Key, Column(Order = 1)]
        public long MenuId { get; set; }
        [LibFromSource("ProgInfo", "ProgNm")]
        public string ProgNm { get; set; }
        public string MenuName { get; set; }
        public long PmenuId { get; set; }
        [Key, Column(Order = 2)]
        public string UserId { get; set; }
        public string UserMenuCode { get; set; }
        public LibMenuType MenuType { get; set; }
        [NotMapped]
        public bool Spread { get; set; }
        [NotMapped]
        public int ProgKind { get; set; }
        public override void OnModelBuilder(ModelBuilder builder)
        {
            base.OnModelBuilder(builder);
            builder.Entity<UserMenu>().HasKey(i => new {i.MenuId,i.UserId });
        }
    }

    public enum LibMenuType
    {
        None=0,
        Prog=1
    }
}
