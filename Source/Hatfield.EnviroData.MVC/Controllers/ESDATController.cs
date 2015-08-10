using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Hatfield.EnviroData.MVC.Controllers
{
    public class ESDATController : Controller
    {
        //
        // GET: /ESDAT/

        public ActionResult Index()
        {
            ViewBag.Title = "Data Acquisition";
            return View();
        }

        public ActionResult ImportFromLocal()
        {
            ViewBag.Title = "Data Acquisition";
            return View();
        }

        public ActionResult ImportFromHttp()
        {
            ViewBag.Title = "Data Acquisition";
            return View();
        }

        public ActionResult ImportFromFtp()
        {
            ViewBag.Title = "Data Acquisition";
            return View();
        }

        public ActionResult ViewImportedData()
        {
            return View();
        }

        public ActionResult ViewDataDetail(int Id)
        {

            return View(Id);
        }

        public ActionResult EditSampleData(int Id)
        {
            return View(Id);
        }

    }
}
