﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Hatfield.EnviroData.Core;

namespace Hatfield.EnviroData.MVC.Controllers
{
    public class HomeController : Controller
    {
        public HomeController()
        {
            
        }

        public ActionResult Index()
        {            
            return View();
        }
    }
}
