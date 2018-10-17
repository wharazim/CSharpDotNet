using MvcPartialViews.Models;
using MvcPartialViews.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcPartialViews.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var vm = new BigBadVoodooViewModel();

            return View(vm);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public JsonResult AddResidence(Residence residence)
        {
            return Json(new { success = true });
        }

        public ActionResult ResidenceList()
        {
            return View("Residence/List", new List<Residence>());
        }
    }
}