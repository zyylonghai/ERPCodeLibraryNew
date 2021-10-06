using System;
using System.Collections.Generic;
using System.Text;

namespace ErpModels.AppDataLog
{
    public class DataLogsD
    {
        public long ID { get; set; }
        public string Action { get; set; }

        public string UserId { get; set; }
        public string IP { get; set; }

        public DateTime DT { get; set; }
        public string Datajson { get; set; }
    }
}
