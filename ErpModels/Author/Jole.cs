using ErpCode.Com.Enums;
using Library.Core.LibAttribute;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ErpModels.Author
{
    /// <summary>
    /// 角色
    /// </summary>
    public class Jole:LibModelCore
    {
        [Key]
        public string JoleId { get; set; }

        public string JoleNm { get; set; }
        public LibStatus Status { get; set; }
        /// <summary>是否管理员角色</summary>
        public bool IsAdminJole { get; set; }

        public string Remark { get; set; }

        public override void OnModelBuilder(ModelBuilder builder)
        {
            base.OnModelBuilder(builder);
            builder.Entity<Jole>().HasKey(i =>new { i.JoleId });
        }


    }
    /// <summary>
    /// 角色明细
    /// </summary>
    public class JoleD : LibModelCore
    {
        [Key, Column(Order = 1)]
        public string JoleId { get; set; }

        [Key, Column(Order = 2)]
        [LibIdentity(1)]
        public int SeqNo { get; set; }
        
        [LibFromSource("ProgInfo", "ProgNm")]
        public string ProgNm { get; set; }

        public string Remark { get; set; }
        public override void OnModelBuilder(ModelBuilder builder)
        {
            base.OnModelBuilder(builder);
            builder.Entity<JoleD>().HasKey(i => new { i.JoleId, i.SeqNo });//设置主键
        }

    }
    /// <summary>
    /// 权限对象
    /// </summary>
    public class AuthorityObj : LibModelCore
    {
        public string JoleId { get; set; }

        public string ProgNm { get; set; }
        public int SeqNo { get; set; }
        public LibAuthorityType AuthorityType { get; set; }
        public string ControlID { get; set; }
        public string Field { get; set; }
        public bool IsHide { get; set; }
        public bool OnlyRead { get; set; }
        [NotMapped]
        public string Title { get; set; }
        public override void OnModelBuilder(ModelBuilder builder)
        {
            base.OnModelBuilder(builder);
            builder.Entity<AuthorityObj>().HasKey(i => new { i.JoleId, i.ProgNm, i.SeqNo });
        }
    }
}
