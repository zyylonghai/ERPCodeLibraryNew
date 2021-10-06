using ErpModels.Com;
using System;
using System.Collections.Generic;
using System.Text;
using ErpRptModels.Com;

namespace LibDBContext.RptDataSource
{
    public class CheckRptDS: RptBaseDataSource
    {
        public CheckRptHeard checkBill { get; set; }
        public CheckRptData CheckRptData { get; set; }
    }
}
