using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Hatfield.EnviroData.MVCPrototype.Controllers.API
{
    public class Site
    {
        public int Id { get; set; }
        public string WaterBodyName { get; set; }
        public string SiteId { get; set; }
        public string ShortSiteDescription { get; set; }
        public float Latitude { get; set; }
        public float Longitude { get; set; }
        public string SiteType { get; set; }
    }

    
    public class WQAPIController : ApiController
    {

        [Route("WQ/QueryData")]
        [HttpGet]
        public HttpResponseMessage Get()
        {
            //Note: Instead of using Object, I should create classes to represent sites, analytes, guidelines, etc....
            var response = new HttpResponseMessage();
            string sitePath = HttpContext.Current.Server.MapPath("~/assets/site.json");
            string siteText = System.IO.File.ReadAllText(sitePath);
            var sites = JsonConvert.DeserializeObject<IEnumerable<Site>>(siteText);
            string analytePath = HttpContext.Current.Server.MapPath("~/assets/analyte.json");
            string analyteText = System.IO.File.ReadAllText(analytePath);
            var analytes = JsonConvert.DeserializeObject<IEnumerable<Object>>(analyteText);
            string guidelinePath = HttpContext.Current.Server.MapPath("~/assets/guideline.json");
            string guidelineText = System.IO.File.ReadAllText(guidelinePath);
            var guidelines = JsonConvert.DeserializeObject<IEnumerable<Object>>(guidelineText);

            var querySites = from site in sites
                                       where site.SiteType == "Surface Water"
                                       select site;

            string jsonResponse = Newtonsoft.Json.JsonConvert.SerializeObject(new { sites = querySites, analytes = analytes, guidelines = guidelines});
            response.Content = new StringContent(jsonResponse);
            return response;
        }

        [Route("WQ/FilterQueryForm")] //this route needs to be renamed, along with the other routes
        [HttpGet]
        public HttpResponseMessage GetFilteredQueryForm([FromBody]Object filterParameters)
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
