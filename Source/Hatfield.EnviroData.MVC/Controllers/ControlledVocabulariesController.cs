using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Hatfield.EnviroData.MVC.Controllers
{
    public class ControlledVocabulariesController : Controller
    {
        //
        // GET: /ControlledVocabularies/

        public ActionResult Index()
        {
            var dateFile = "\\date.json";
            string lastRunTime = null;
            var baseDir = AppDomain.CurrentDomain.BaseDirectory;
            string filePath = baseDir + dateFile;
            //Debugger.Launch();
            if (System.IO.File.Exists(filePath))
            {
                var file = new StreamReader(filePath);
                var json = file.ReadToEnd();
                file.Close();
                if (json != "")
                {
                    lastRunTime = Convert.ToDateTime(JsonConvert.DeserializeObject(json)).ToString("MMM-dd-yyyy, HH:mm tt");
                }
            }
            ViewBag.LastRunTime = lastRunTime;

            ViewBag.Title = "Controlled Vocabularies";
            return View();
        }

    }
}
