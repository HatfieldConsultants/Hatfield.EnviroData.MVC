using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Hatfield.EnviroData.Core;
using System.Configuration;
using Hatfield.EnviroData.CVUpdater;
using System.Xml.Linq;

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

        public ActionResult CVUpdates()
        {
            string ApiUrl = ConfigurationManager.AppSettings["ApiUrl"];
            string VocabSiteUrl = ConfigurationManager.AppSettings["VocabTermsUrl"];

            CVTermAPILayer parser = new CVTermAPILayer();
            CVTermBusinessLayer biz = new CVTermBusinessLayer(new ODM2Entities());

            var endpoints = parser.GetAPIEndpoints(VocabSiteUrl);

            //Get data for each CV Type, extract and write to the DB
            foreach (var endpoint in endpoints)
            {
                var doc = new XDocument();
                var rawCV = parser.GetSingleCV(ApiUrl, endpoint.Value, "skos");
                var results = parser.ImportXMLData(XDocument.Parse(rawCV));
                biz.AddOrUpdateCVs(endpoint.Value, results.ExtractedEntities);
                biz.CheckForDeleted(endpoint.Value, results.ExtractedEntities);

            }
            return new EmptyResult();
        }
    }
}
