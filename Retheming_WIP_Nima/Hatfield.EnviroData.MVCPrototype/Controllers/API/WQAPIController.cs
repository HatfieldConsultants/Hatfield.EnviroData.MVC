using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;

using System.Diagnostics;

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

    public class DataCollection
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
        public string modifiedFormId { get; set; }
        public List<Site> formSites { get; set; }
        public List<Analyte> formAnalytes { get; set; }
        public List<Guideline> formGuidelines { get; set; }
    }

    public class WQAPIController : ApiController
    {

        /*[Route("WQ/QudasderyData")]
        [HttpPost]
        public HttpResponseMessage PostOld([FromBody] QueryForm queryParams)
        {
            var response = new HttpResponseMessage();

            //Load all data which will then be queried
            string sitePath = HttpContext.Current.Server.MapPath("~/assets/site.json");
            string siteText = System.IO.File.ReadAllText(sitePath);
            var sites = JsonConvert.DeserializeObject<List<Site>>(siteText);
            string analytePath = HttpContext.Current.Server.MapPath("~/assets/analyte.json");
            string analyteText = System.IO.File.ReadAllText(analytePath);
            var analytes = JsonConvert.DeserializeObject<List<Analyte>>(analyteText);
            string guidelinePath = HttpContext.Current.Server.MapPath("~/assets/guideline.json");
            string guidelineText = System.IO.File.ReadAllText(guidelinePath);
            var guidelines = JsonConvert.DeserializeObject<List<Guideline>>(guidelineText);
            string dataPath = HttpContext.Current.Server.MapPath("~/assets/sampleData.json");
            string dataText = System.IO.File.ReadAllText(dataPath);
            var dataCollection = JsonConvert.DeserializeObject<List<DataCollection>>(dataText);
            string standardPath = HttpContext.Current.Server.MapPath("~/assets/standard.json");
            string standardText = System.IO.File.ReadAllText(standardPath);
            var standards = JsonConvert.DeserializeObject<List<Standard>>(standardText);

            // join everything into one big table
            // bad performance, good for now though probably

            var bigLookup = from datum in dataCollection
                            join site in sites on datum.ClientSampleID equals site.SiteId
                            join analyte in analytes on datum.WaterQualityLabAnalyteId equals analyte.Id
                            join standard in standards on analyte.Id equals standard.WaterQualityLabAnalyteId
                            join guideline in guidelines on standard.GuidelineId equals guideline.Id
                            select new { SiteId = site.SiteId, SiteType = site.SiteType, AnalyteName = analyte.AnalyteName, Id = analyte.Id, GuidelineName = guideline.GuidelineName };

            // every form has to be done manually right now. 

            var returnAnalytes = new List<Analyte>();
            var returnAnalytes_SiteFilter = new List<Analyte>();
            var returnAnalytes_GuidelineFilter = new List<Analyte>();

            // Parameter Filtering
            // Filter by Site            
            if (!String.IsNullOrEmpty(queryParams.selectedSites))
            {
                var siteArray = Regex.Split(queryParams.selectedSites, ",");
                Debug.WriteLine(string.Join(",", siteArray));
                var queryAnalytes = (from entry in bigLookup
                                     where siteArray.Contains(entry.SiteId)
                                     select new { AnalyteName = entry.AnalyteName, Id = entry.Id }).GroupBy(x => x.AnalyteName).Select(y => y.First());
                foreach (var analyte in queryAnalytes)
                {
                    returnAnalytes_SiteFilter.Add(new Analyte { AnalyteName = analyte.AnalyteName, Id = analyte.Id });
                }
            }
            // Filter by Guidelines
            if (!String.IsNullOrEmpty(queryParams.selectedGuidelines))
            {
                var guidelineArray = Regex.Split(queryParams.selectedGuidelines, ",");
                var queryAnalytes = (from entry in bigLookup
                                     where guidelineArray.Contains(entry.GuidelineName)
                                     select new { AnalyteName = entry.AnalyteName, Id = entry.Id }).GroupBy(x => x.AnalyteName).Select(y => y.First());
                foreach (var analyte in queryAnalytes)
                {
                    returnAnalytes_GuidelineFilter.Add(new Analyte { AnalyteName = analyte.AnalyteName, Id = analyte.Id });
                }

                //Intersect the two lists

                if (returnAnalytes_SiteFilter.Count > 0 && returnAnalytes_GuidelineFilter.Count > 0)
                {
                    returnAnalytes = returnAnalytes_SiteFilter.Intersect(returnAnalytes_GuidelineFilter).ToList();
                }
                else
                {
                    returnAnalytes = returnAnalytes_SiteFilter.Concat(returnAnalytes_GuidelineFilter).ToList();
                };
            }
            else
            {
                returnAnalytes = analytes; //if no guidelines selected, retrn all analytes
            };

            string jsonResponse = Newtonsoft.Json.JsonConvert.SerializeObject(new { sites = sites, analytes = returnAnalytes, guidelines = guidelines, message = queryParams.modifiedFormId });
            response.Content = new StringContent(jsonResponse);

            return response;

        } */



        [Route("WQ/QueryData")]
        [HttpPost]
        public HttpResponseMessage Post([FromBody] QueryForm queryParams)
        {
            var response = new HttpResponseMessage();

            //Load all data which will then be queried
            string sitePath = HttpContext.Current.Server.MapPath("~/assets/site.json");
            var siteText = JsonConvert.DeserializeObject(System.IO.File.ReadAllText(sitePath));
            string analytePath = HttpContext.Current.Server.MapPath("~/assets/analyte.json");
            var analyteText = JsonConvert.DeserializeObject(System.IO.File.ReadAllText(analytePath));
            string guidelinePath = HttpContext.Current.Server.MapPath("~/assets/guideline.json");
            var guidelineText = JsonConvert.DeserializeObject(System.IO.File.ReadAllText(guidelinePath));


            string jsonResponse = Newtonsoft.Json.JsonConvert.SerializeObject(new { sites = siteText, analytes = analyteText, guidelines = guidelineText });
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
            var siteText = JsonConvert.DeserializeObject(System.IO.File.ReadAllText(sitePath));
            string analytePath = HttpContext.Current.Server.MapPath("~/assets/analyte.json");
            var analyteText = JsonConvert.DeserializeObject(System.IO.File.ReadAllText(analytePath));
            string guidelinePath = HttpContext.Current.Server.MapPath("~/assets/guideline.json");
            var guidelineText = JsonConvert.DeserializeObject(System.IO.File.ReadAllText(guidelinePath));


            string jsonResponse = Newtonsoft.Json.JsonConvert.SerializeObject(new { sites = siteText, analytes = analyteText, guidelines = guidelineText });
            response.Content = new StringContent(jsonResponse);
            return response;
        }

    }
}
