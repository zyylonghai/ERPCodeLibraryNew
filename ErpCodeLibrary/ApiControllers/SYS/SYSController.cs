using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ErpCode.BaseApiController;
using ErpCode.Com;
using ErpCode.comDal;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ErpCodeLibrary.ApiControllers.SYS
{
    [Route("SYS/[action]/{prog}")]
    [ApiController]
    [Authorize]
    public class SYSController :DataBaseController<COMDal>
    {
        /// <summary>根据用户输入的功能代码查找功能</summary>
        /// <param name="code"></param>
        /// <returns></returns>
        [HttpGet]
        public ResponseMsg GetUserMenuByUserCode(string code)
        {
            var menu = this.tDal.GetUserMenuByUserCode(code);
            return new ResponseMsg { IsSuccess = true, Data = menu };
        }
    }
}