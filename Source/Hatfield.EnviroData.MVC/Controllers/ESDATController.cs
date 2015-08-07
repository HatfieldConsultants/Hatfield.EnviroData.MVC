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
            return View();
        }

        public ActionResult ImportFromLocal()
        {
            return View();
        }

        public ActionResult ImportFromHttp()
        {
            return View();
        }

        public ActionResult ImportFromFtp()
        {
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
