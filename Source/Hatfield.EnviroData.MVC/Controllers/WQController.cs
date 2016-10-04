using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Hatfield.EnviroData.MVC.Controllers
{
    public class WQController : Controller
    {
        // GET: WQ
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Query()
        {
            return View();
        }

        public ActionResult DataFiles_List()
        {
            return View();
        }

        public ActionResult Samples_List()
        {
            return View();
        }

        public ActionResult Sample_Details()
        {
            return View();
        }

        public ActionResult CustomReport()
        {
            return View();
        }
    }
}