using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Hatfield.EnviroData.MVC.Controllers
{
    public class ManageController : Controller
    {
        //
        // GET: /Manage/

        public ActionResult Index()
        {
            ViewBag.Title = "Manage Data";
            return View();
        }

        [HttpGet]
        public ActionResult Sites()
        {
            ViewBag.Title = "Manage Sites";
            return View();
        }

        [HttpGet]
        public ActionResult Projects()
        {
            ViewBag.Title = "Manage Projects";
            return View();
        }

        [HttpGet]
        public ActionResult OrganizationsAndPeople()
        {
            ViewBag.Title = "Manage Organizations and People";
            return View();
        }

        [HttpGet]
        public ActionResult Equipment()
        {
            ViewBag.Title = "Manage Equipment";
            return View();
        }

        [HttpGet]
        public ActionResult Simulations()
        {
            ViewBag.Title = "Manage Simulations";
            return View();
        }

        [HttpGet]
        public ActionResult ReferenceMaterialsAndCitations()
        {
            ViewBag.Title = "Manage Reference Materials and Citations";
            return View();
        }

        [HttpGet]
        public ActionResult CVs()
        {
            ViewBag.Title = "View Controlled Vocabularies";
            return View();
        }


    }
}
