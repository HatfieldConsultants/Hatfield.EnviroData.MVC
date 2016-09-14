using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Hatfield.EnviroData.MVCPrototype.Controllers
{
    public class CoreDataController : Controller
    {
        // GET: CoreData
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult MonitoringSites_List()
        {
            return View();
        }

        public ActionResult MonitoringSites_Map()
        {
            return View();
        }

        public ActionResult MonitoringSite_Details()
        {
            return View();
        }

        public ActionResult Analytes_List()
        {
            return View();
        }

        public ActionResult Guidelines_List()
        {
            return View();
        }

        public ActionResult Projects_List()
        {
            return View();
        }

        public ActionResult Project_Details()
        {
            return View();
        }
    }
}