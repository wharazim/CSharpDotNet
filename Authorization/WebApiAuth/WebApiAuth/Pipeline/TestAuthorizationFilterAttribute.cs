using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace WebApiAuth.Pipeline
{
    public class TestAuthorizationFilterAttribute : AuthorizeAttribute
    {
        protected override bool IsAuthorized(HttpActionContext actionContext)
        {
            Logger.Write("AuthorizationFilter", actionContext.RequestContext.Principal);
            //return base.IsAuthorized(actionContext);
            return true;
        }
    }
}