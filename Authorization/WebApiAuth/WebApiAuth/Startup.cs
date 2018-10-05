using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using WebApiAuth.Pipeline;

namespace WebApiAuth
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var configuration = new HttpConfiguration();
            configuration.Routes.MapHttpRoute(
                "default",
                "api/{controller}");

            app.Use(typeof(TestMiddleware));
            app.UseWebApi(configuration);
        }
    }
}