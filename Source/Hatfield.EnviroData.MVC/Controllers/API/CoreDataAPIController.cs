using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


namespace Hatfield.EnviroData.MVC.Controllers.API
{
    public class CoreDataAPIController : ApiController
    {
        [Route("CoreData/MonitoringSites")]
        [HttpGet]
        public HttpResponseMessage Get()
        {
            var response = new HttpResponseMessage();
            string sitePath = HttpContext.Current.Server.MapPath("~/assets/site.json");
            string siteText = System.IO.File.ReadAllText(sitePath);
            string analytePath = HttpContext.Current.Server.MapPath("~/assets/analyte.json");
            string analyteText = System.IO.File.ReadAllText(analytePath);
            string guidelinePath = HttpContext.Current.Server.MapPath("~/assets/guideline.json");
            string guidelineText = System.IO.File.ReadAllText(guidelinePath);
            string jsonResponse = Newtonsoft.Json.JsonConvert.SerializeObject(new { sites = siteText, analytes = analyteText, guidelines = guidelineText });
            response.Content = new StringContent(jsonResponse);
            return response;
        }

    }
}
