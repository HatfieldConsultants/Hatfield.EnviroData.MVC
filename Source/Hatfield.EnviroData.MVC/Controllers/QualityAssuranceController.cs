﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Hatfield.EnviroData.MVC.Controllers
{
    public class QualityAssuranceController : Controller
    {
        //
        // GET: /QualityAssurance/

        public ActionResult Index()
        {
            ViewBag.Title = "Quality Assurance";
            return View();
        }

        public ActionResult Detail(int Id)
        {
            return View();
        }

    }
}
