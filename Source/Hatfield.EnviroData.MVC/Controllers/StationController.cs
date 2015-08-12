using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;

using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;

using Hatfield.EnviroData.MVC.Helpers;
using Hatfield.EnviroData.MVC.Models;

namespace Hatfield.EnviroData.MVC.Controllers
{
    public class StationController : Controller
    {
        //
        // GET: /Station/

        public ActionResult Index()
        {            
            ViewBag.Title = "Query Water Quality Data";
            return View();
        }

        

    }
}
