using Library.Core.LibAttribute;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ErpModels.Com
{
    public class CodeRuleConfig : LibModelCore
    {
        [Key, Column(Order = 1)]
        [LibFromSource("ProgInfo", "ProgNm")]
        public string ProgNm { get; set; }

        [Key, Column(Order = 2)]
        [LibFromSource("CodeRule", "RuleId")]
        public string RuleId { get; set; }

        public string CurrDate { get; set; }

        public int CurrSerial { get; set; }

        public string GroupNo { get; set; }

        public bool IsDefault { get; set; }

        public override void OnModelBuilder(ModelBuilder builder)
        {
            base.OnModelBuilder(builder);
            builder.Entity<CodeRuleConfig>().HasKey(i => new {i.ProgNm ,i.RuleId });
        }
    }
}
