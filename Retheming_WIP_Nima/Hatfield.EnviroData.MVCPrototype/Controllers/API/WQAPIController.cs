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

    public class Analyte
    {
        public int Id { get; set; }
        public int LabId { get; set; }
        public string AnalyteName { get; set; }
        public int NumberOfDecimalPlaces { get; set; }
        public int CategoryId { get; set; }
        public double DetectionLimit { get; set; }
        public string Unit { get; set; }
    }

    public class Guideline
    {
        public int Id { get; set; }
        public string GuidelineName { get; set; }
        public string GuidelineLongName { get; set; }
    }

    public class Data
    {
        public int Id { get; set; }
        public string LabSampleID { get; set; }
        public string ClientSampleID { get; set; }
        public int WaterQualityLabAnalyteId { get; set; }
        public DateTime SampleDateTime { get; set; }
        public string Units { get; set; }
        public double Result { get; set; }
        public string ResultQualifier { get; set; }
        public double DetectionLimit { get; set; }
        public int LabId { get; set; }
        public string SampleType { get; set; }
        public string Method { get; set; }
        public int VMVCode { get; set; }
        public int StationId { get; set; }
    }

    public class Standard
    {
        public int Id { get; set; }
        public string AnalyteNameInGuideline { get; set; }
        public int GuidelineId { get; set; }
        public int WaterQualityLabAnalyteId { get; set; }
        public string Unit { get; set; }
        public string StandardValue { get; set; }
        public string ValueType { get; set; }
        public double ThresholdValue { get; set; }
        public string ReferenceSource { get; set; }
    }

    public class QueryForm
    {
        public IEnumerable<Analyte> visibleAnalytes { get; set; }
        public IEnumerable<Guideline> visibleGuidelines { get; set; }
        public IEnumerable<Site> visibleSites { get; set; }
        public string modifiedFormId { get; set; }
    }

    public class WQAPIController : ApiController
    {

        [Route("WQ/QueryData")]
        [HttpPost]
        public HttpResponseMessage Post([FromBody] QueryForm queryParams)
        {
            var response = new HttpResponseMessage();
            
            //Load all data which will then be queried
            string sitePath = HttpContext.Current.Server.MapPath("~/assets/site.json");
            string siteText = System.IO.File.ReadAllText(sitePath);
            var sites = JsonConvert.DeserializeObject<IEnumerable<Site>>(siteText);
            string analytePath = HttpContext.Current.Server.MapPath("~/assets/analyte.json");
            string analyteText = System.IO.File.ReadAllText(analytePath);
            var analytes = JsonConvert.DeserializeObject<IEnumerable<Analyte>>(analyteText);
            string guidelinePath = HttpContext.Current.Server.MapPath("~/assets/guideline.json");
            string guidelineText = System.IO.File.ReadAllText(guidelinePath);
            var guidelines = JsonConvert.DeserializeObject<IEnumerable<Guideline>>(guidelineText);
            string dataPath = HttpContext.Current.Server.MapPath("~/assets/sampleData.json");
            string dataText = System.IO.File.ReadAllText(dataPath);
            var data = JsonConvert.DeserializeObject<IEnumerable<Data>>(dataText);
            string standardPath = HttpContext.Current.Server.MapPath("~/assets/standard.json");
            string standardText = System.IO.File.ReadAllText(standardPath);
            var standards = JsonConvert.DeserializeObject<IEnumerable<Standard>>(standardText);


            //test queries
            //var querySites = from site in sites
              //               select site;

            var querySites = sites;

            var queryAnalytes = (from analyte in analytes
                                select analyte);

            var queryGuidelines = from guideline in guidelines
                                   select guideline;

            
            string jsonResponse = Newtonsoft.Json.JsonConvert.SerializeObject(new { sites = querySites, analytes = queryAnalytes, guidelines = queryGuidelines, message = queryParams.modifiedFormId });
            response.Content = new StringContent(jsonResponse);

            return response;
        }
        [Route("WQ/LoadForm")]
        [HttpGet]
        public HttpResponseMessage Get()
        {
            var response = new HttpResponseMessage();

            //Load all data which will then be queried
            string sitePath = HttpContext.Current.Server.MapPath("~/assets/site.json");
            string siteText = System.IO.File.ReadAllText(sitePath);
            string analytePath = HttpContext.Current.Server.MapPath("~/assets/analyte.json");
            string analyteText = System.IO.File.ReadAllText(analytePath);
            string guidelinePath = HttpContext.Current.Server.MapPath("~/assets/guideline.json");
            string guidelineText = System.IO.File.ReadAllText(guidelinePath);
           

            string jsonResponse = Newtonsoft.Json.JsonConvert.SerializeObject(new { sites = siteText, analytes = analyteText, guidelines = guidelineText});
            response.Content = new StringContent(jsonResponse);
            return response;
        }

    }
}
