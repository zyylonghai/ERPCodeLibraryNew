using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ErpModels.Appsys
{
    public class ProgInfo : LibModelCore
    {
        /// <summary>
        /// 功能名（主键)
        /// </summary>
        [Key]
        public string progNm { get; set; }

        /// <summary>
        /// 功能描述
        /// </summary>
        public string progDesc { get; set; }
        /// <summary>
        /// 功能所属包
        /// </summary>
        public string progPackage { get; set; }

        public string mastTable { get; set; }

        /// <summary>
        /// APIController名称
        /// </summary>
        public string controllerNm { get; set; }

        public string JsUrl { get; set; }
        /// <summary>自定义页面</summary>
        public string CustomPage { get; set; }
        /// <summary>是否挂菜单</summary>
        public bool IsMenu { get; set; }
        /// <summary> 禁用新增按钮</summary>
        public bool HasAddbtn { get; set; }
        /// <summary> 禁用编辑按钮</summary>
        public bool Haseditbtn { get; set; }
        /// <summary>禁用删除按钮</summary>
        public bool Hasdeletebtn { get; set; }
        /// <summary>禁用复制按钮</summary>
        public bool HasCopybtn { get; set; }
        /// <summary>禁用查询按钮</summary>
        public bool HasSearchbtn { get; set; }
        /// <summary> 禁用日志查询按钮</summary>
        public bool HasLogSearchbtn { get; set; }

        /// <summary>功能种类</summary>
        public ProgKind ProgKind { get; set; }

        /// <summary>是否开发者功能,默认false</summary>
        [NotMapped]
        public bool IsDeveloper { get; set; }

        public override void OnModelBuilder(ModelBuilder builder)
        {
            base.OnModelBuilder(builder);
            builder.Entity<ProgInfo>().HasKey(i => new { i.progNm, i.ClientId });
        }

    }

    public class RptHtmlInfo : LibModelCore
    {
        /// <summary>
        /// 功能名（主键)
        /// </summary>
        [Key]
        public string progNm { get; set; }
        public string HtmlStr { get; set; }
    }

    /// <summary>功能种类</summary>
    public enum ProgKind
    {
        /// <summary>普通数据维护功能</summary>
        Ordinary=1,
        /// <summary>A4报表功能</summary>
        A4Rpt=2,
        /// <summary>搜索类报表</summary>
        SearchRpt=3
    }
}
