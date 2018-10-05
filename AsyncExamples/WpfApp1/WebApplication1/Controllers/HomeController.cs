using SecurityPipeline;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        public async Task<ActionResult> Index()
        {
            using (var client = new HttpClient())
            {
                // configure await = false here is used becuase we don't care which thread the async mehod executes on
                // this allows the OS to pick the best thread from the thread pool instead of waiting for the main to become available
                var httpMessage = await client.GetAsync("http://www.berkone.com/").ConfigureAwait(false);
                var content = await httpMessage.Content.ReadAsStringAsync().ConfigureAwait(false);

                return Content(content);
            }
        }

        public ActionResult About()
        {
            Logger.Write("About controller", User);
            ViewBag.Message = "Your application description page.";

            return View();
        }

        [Authorize]
        public ActionResult Contact()
        {
            Logger.Write("Contact controller", User);
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}