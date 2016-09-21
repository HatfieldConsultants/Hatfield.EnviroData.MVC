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
        public bool selected { get; set; }
        public bool visible { get; set; }
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
        public bool selected { get; set; }
        public bool visible { get; set; }
    }

    public class Guideline
    {
        public int Id { get; set; }
        public string GuidelineName { get; set; }
        public string GuidelineLongName { get; set; }
        public bool selected { get; set; }
        public bool visible { get; set; }
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
        public List<string> selectedSites { get; set; }
        public List<string> selectedAnalytes { get; set; }
        public List<string> selectedGuidelines { get; set; }
        public List<string> hiddenSites { get; set; }
        public List<int> hiddenAnalytes { get; set; }
        public List<string> hiddenGuidelines { get; set; }
    }

    public class WQAPIController : ApiController
    {


        [Route("WQ/QueryData")]
        [HttpPost]
        public HttpResponseMessage Post([FromBody] QueryForm queryParams)
        {
            var response = new HttpResponseMessage();

            //Load all data which will then be queried
            //In the real version, this function will instead make SQL queries to the database.
            //But for now I need the data somewhere.
            string sitePath = HttpContext.Current.Server.MapPath("~/assets/site.json");
            var sites = JsonConvert.DeserializeObject<List<Site>>(System.IO.File.ReadAllText(sitePath));
            string analytePath = HttpContext.Current.Server.MapPath("~/assets/analyte.json");
            var analytes = JsonConvert.DeserializeObject<List<Analyte>>(System.IO.File.ReadAllText(analytePath));
            string guidelinePath = HttpContext.Current.Server.MapPath("~/assets/guideline.json");
            var guidelines = JsonConvert.DeserializeObject<List<Guideline>>(System.IO.File.ReadAllText(guidelinePath));
            string dataPath = HttpContext.Current.Server.MapPath("~/assets/sampleData.json");
            string dataText = System.IO.File.ReadAllText(dataPath);
            var data = JsonConvert.DeserializeObject<List<DataCollection>>(dataText);
            string standardPath = HttpContext.Current.Server.MapPath("~/assets/standard.json");
            string standardText = System.IO.File.ReadAllText(standardPath);
            var standards = JsonConvert.DeserializeObject<List<Standard>>(standardText);

            var hiddenSites = new List<string>();
            var hiddenAnalytes = new List<int>(); //queryParams.hiddenAnalytes;
            var hiddenGuidelines = new List<string>();

            //for some reason, analytes works without having to do this... but it adds a 0. So..... I need to figure out
            //why that extra 0 is added, and 
            if (queryParams.hiddenSites[0] != null)
            {
                hiddenSites = Regex.Split(queryParams.hiddenSites[0], ",").ToList<string>();
            }
            if (queryParams.hiddenGuidelines[0] != null)
            {
                hiddenGuidelines = Regex.Split(queryParams.hiddenGuidelines[0], ",").ToList<string>();
            }
            
            var bigLookupQuery =
                from datum in data
                join site in sites on new { siteId = datum.ClientSampleID } equals new { siteId = site.SiteId }
                join analyte in analytes on new { analyteId = datum.WaterQualityLabAnalyteId } equals new { analyteId = analyte.Id }
                join standard in standards on new { analyteId = analyte.Id } equals new { analyteId = standard.WaterQualityLabAnalyteId }
                join guideline in guidelines on new { guidelineId = standard.GuidelineId } equals new { guidelineId = guideline.Id }
                select new { siteId = site.SiteId, analyteId = analyte.Id, guidelineName = guideline.GuidelineName };

            if (queryParams.modifiedFormId != "sites")
            {
                var distinctSites =
                    from row in bigLookupQuery
                    where !Regex.Split(queryParams.selectedSites[0], ",").ToList<string>().Contains(row.siteId)
                    group row by row.siteId
                    into sortedRows
                    select sortedRows.FirstOrDefault();

                foreach (var site in distinctSites)
                {
                    hiddenSites.Add(site.siteId);
                }
            }

            string jsonResponse = Newtonsoft.Json.JsonConvert.SerializeObject(new { hiddenSites = hiddenSites, hiddenAnalytes = hiddenAnalytes, hiddenGuidelines = hiddenGuidelines });
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
