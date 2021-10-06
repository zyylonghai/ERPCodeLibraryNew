using ErpCode.BaseApiController;
using Microsoft.AspNetCore.Mvc;
using System;
using WMS.MasterDataDal;

namespace WMS.MasterDataController
{
    [Route("WMSMaster/[controller]/[action]/{prog}")]
    [ApiController]
    public class MaterialController: DataBaseController<DataMasterDal>
    {
        protected override void PageLoadExt()
        {
            base.PageLoadExt();
        }
    }
}
