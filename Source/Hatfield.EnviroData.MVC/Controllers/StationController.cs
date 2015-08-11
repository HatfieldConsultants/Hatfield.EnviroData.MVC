using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using OfficeOpenXml;

using Hatfield.EnviroData.MVC.Helpers;

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

        public FileResult DownloadQueryData()
        {
            var spreadSheet = SpreadsheetHelper.GenerateQueryDataResultSpreadshet("Test");
            var byteArray= spreadSheet.GetAsByteArray();
            return File(byteArray, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "TestFile.xlsx");
        }

    }
}
