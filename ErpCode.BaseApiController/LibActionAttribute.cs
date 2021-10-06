using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Text;

namespace ErpCode.BaseApiController
{
    public class LibActionAttribute : ActionFilterAttribute
    {
        public LibActionAttribute()
        {
            
        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            base.OnActionExecuted(context);
        }
    }
}
