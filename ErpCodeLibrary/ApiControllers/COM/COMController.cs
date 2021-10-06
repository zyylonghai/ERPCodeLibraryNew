using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ErpCode.AppDal;
using ErpCode.BaseApiController;
using ErpCode.Com;
using ErpCode.comDal;
using ErpModels.Com;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ErpCodeLibrary.ApiControllers.COM
{
    [Route("COM/[action]/{prog}")]
    [ApiController]
    [Authorize]
    public class COMController : DataBaseController<COMDal>
    {
        
    }
}