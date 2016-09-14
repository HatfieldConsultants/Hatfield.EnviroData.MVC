using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Hatfield.EnviroData.MVCPrototype.Controllers
{
    public class AdminController : Controller
    {
        // GET: Admin
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Guidelines_List()
        {
            return View();
        }

        public ActionResult MonitoringComponents_List()
        {
            return View();
        }

        public ActionResult MonitoringComponent_Create()
        {
            return View();
        }

        public ActionResult Logs()
        {
            return View();
        }

        public ActionResult Users_List()
        {
            return View();
        }
    }
}