using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using ErpCode.Com.Enums;
using Microsoft.EntityFrameworkCore;

namespace ErpModels.Appsys
{
    public class LanguageField : LibModelCore
    {
        [Key, Column(Order = 1)]
        public LibLanguage LanguageId { get; set; }
        [Key, Column(Order = 2)]
        public string ProgNm { get; set;  }
        [Key, Column(Order = 3)]
        public string FieldNm { get; set; }
        [Key, Column(Order = 4)]
        public string TableNm { get; set; }
        public string Vals { get; set; }
        public override void OnModelBuilder(ModelBuilder builder)
        {
            base.OnModelBuilder(builder);
            builder.Entity<LanguageField>().HasKey(i => new { i.LanguageId ,i.ProgNm ,i.FieldNm ,i.TableNm,i.ClientId });
        }
    }
}
