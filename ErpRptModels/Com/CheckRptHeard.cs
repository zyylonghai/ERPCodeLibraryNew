using System;
using System.Collections.Generic;
using System.Text;

namespace ErpRptModels.Com
{
    public class CheckRptHeard:IRptEntityConfig
    {
        public string Title { get; set; }
        public string Imgurl { get; set; }

        public string checker { get; set; }
        public string checkdt { get; set; }
    }
}
