using Library.Core;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ErpModels
{
    public class LibModelCore: IEntityConfigure
    {
        public string ClientId { get; set; }
        public DateTime? CreateDT { get; set; }
        public string Creater { get; set; }
        public DateTime? LastModifyDT { get; set; }
        public string LastModifier { get; set; }

        public bool IsDeleted { get; set; }

        public string app_logid { get; set; }

        [NotMapped]
        public LibModelStatus LibModelStatus { get; set; }
        [NotMapped]
        public bool LAY_CHECKED { get; set; }

        public LibModelCore()
        {
            if (string.IsNullOrEmpty(app_logid))
            {
                
                app_logid = TimestampID.GetInstance().GetID ();
            }
        }
        public virtual void OnModelBuilder(ModelBuilder builder)
        {

        }

    }

    public enum LibModelStatus
    {
        Add=1,
        Edit=2,
        Delete=3,
        /// <summary>修改了主键列的值</summary>
        KeyUpdate=4
    }
}
