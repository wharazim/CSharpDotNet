using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace WebApiAuth.Pipeline
{
    [TestAuthenticationFilter]
    public class TestController : ApiController
    {
        [TestAuthorizationFilter]
        public IHttpActionResult Get()
        {
            Logger.Write("Controller", User);
            return Ok();
        }
    }
}
