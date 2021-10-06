using ErpCode.Com.Enums;
using Library.Core.LibAttribute;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ErpModels.Com
{
    public class CodeRule:LibModelCore 
    {
        [Key]
        public string RuleId { get; set; }
        public string RuleNm { get; set; }
        public LibStatus Status { get; set; }

        public string Remark { get; set; }

        public override void OnModelBuilder(ModelBuilder builder)
        {
            base.OnModelBuilder(builder);
            builder.Entity<CodeRule>().HasKey(i => new {i.RuleId });
        }
    }

    public class CodeRuleD : LibModelCore
    {
        [Key, Column(Order = 1)]
        public string RuleId { get; set; }
        [Key, Column(Order = 2)]
        [LibIdentity(1)]
        public int SeqNo { get; set; }
        public LibModule ModuleId { get; set; }
        public string FixValue { get; set; }
        public int SeriaLen { get; set; }

        public override void OnModelBuilder(ModelBuilder builder)
        {
            base.OnModelBuilder(builder);
            builder.Entity<CodeRuleD>().HasKey(i => new {i.RuleId ,i.SeqNo });
        }
    }

    public enum LibModule
    {
        prefix=1000,
        yyyy =1001,
        MM=1002,
        dd=1003,
        serial=1004,
        suffix=1005,
        yy=1006
    }
}
