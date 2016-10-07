using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;

using System.Diagnostics;

public static class PredicateBuilder
{
    public static Expression<Func<T, bool>> True<T>() { return f => true; }
    public static Expression<Func<T, bool>> False<T>() { return f => false; }

    public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> expr1,
                                                        Expression<Func<T, bool>> expr2)
    {
        var invokedExpr = Expression.Invoke(expr2, expr1.Parameters.Cast<Expression>());
        return Expression.Lambda<Func<T, bool>>
              (Expression.OrElse(expr1.Body, invokedExpr), expr1.Parameters);
    }

    public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> expr1,
                                                         Expression<Func<T, bool>> expr2)
    {
        var invokedExpr = Expression.Invoke(expr2, expr1.Parameters.Cast<Expression>());
        return Expression.Lambda<Func<T, bool>>
              (Expression.AndAlso(expr1.Body, invokedExpr), expr1.Parameters);
    }
}

namespace Hatfield.EnviroData.MVC.Controllers.API
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
        public string selectedSites { get; set; }
        public string selectedAnalytes { get; set; }
        public string selectedGuidelines { get; set; }
        public string hiddenSites { get; set; }
        public string hiddenAnalytes { get; set; }
        public string hiddenGuidelines { get; set; }
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

            var bigLookupQuery =
                from datum in data
                join site in sites on new { siteId = datum.StationId } equals new { siteId = site.Id }
                join analyte in analytes on new { analyteId = datum.WaterQualityLabAnalyteId } equals new { analyteId = analyte.Id }
               // join standard in standards on new { analyteId = analyte.Id } equals new { analyteId = standard.WaterQualityLabAnalyteId }
               // join guideline in guidelines on new { guidelineId = standard.GuidelineId } equals new { guidelineId = guideline.Id }
                select new
                {

                    siteId = datum.StationId,
                    siteName = site.SiteId,
                    siteType = site.SiteType,

                    analyteId = analyte.Id,
                    analyteName = analyte.AnalyteName,

                   // guidelineID = guideline.Id,
                  //  guidelineName = guideline.GuidelineName

                };

            var selectedSites = new string[0];
            var selectedAnalytes = new string[0];
            var selectedGuidelines = new string[0];
            
            Regex regex = new Regex(",");

            if (queryParams.selectedSites != null)
            {
                selectedSites = regex.Split(queryParams.selectedSites);
            }

            if (queryParams.selectedAnalytes != null)
            {
                selectedAnalytes = regex.Split(queryParams.selectedAnalytes);
            }

            if (queryParams.selectedGuidelines != null)
            {
                selectedGuidelines = regex.Split(queryParams.selectedGuidelines);
            }

            var hiddenSites = new List<int>();
            var hiddenAnalytes = new List<int>();
            var hiddenGuidelines = new List<int>();

            if (true)
            {
                var distinctSites =
                    from row in bigLookupQuery
                    group row by row.siteId
                    into sortedRows 
                    select sortedRows.FirstOrDefault();
               
                // all sites connected to selected analytes
                var queryAnalyteSites =
                    from row in bigLookupQuery
                    where selectedAnalytes.Contains(row.analyteId.ToString())
                    group row by row.siteId
                    into sortedRows
                    select sortedRows.FirstOrDefault();

                if (!selectedAnalytes.Any())
                {
                    queryAnalyteSites =
                        from row in bigLookupQuery
                        group row by row.siteId
                        into sortedRows
                        select sortedRows.FirstOrDefault();

                }

                //add all sites
                foreach (var site in distinctSites)
                {
                    hiddenSites.Add(site.siteId);
                }
                //remove selected sites
                foreach (var site in selectedSites)
                {
                    hiddenSites.RemoveAll(item => item == Int32.Parse(site));
                }
                //remove analyte connected sites
                foreach (var site in queryAnalyteSites)
                {
                    hiddenSites.RemoveAll(item => item == site.siteId);
                }
            }

            if (true)
            {
                var distinctAnalytes =
                    from row in bigLookupQuery
                    group row by row.analyteId
                        into sortedRows
                        select sortedRows.FirstOrDefault();

                // all sites connected to selected analytes
                var querySiteAnalytes =
                    from row in bigLookupQuery
                    where selectedSites.Contains(row.siteId.ToString())
                    group row by row.analyteId
                        into sortedRows
                        select sortedRows.FirstOrDefault();

                if (!selectedSites.Any())
                {
                    querySiteAnalytes =
                        from row in bigLookupQuery
                        group row by row.analyteId
                            into sortedRows
                            select sortedRows.FirstOrDefault();

                }

                //add all analytes
                foreach (var analyte in distinctAnalytes)
                {
                    hiddenAnalytes.Add(analyte.analyteId);
                }
                //remove selected analytes
                foreach (var analyte in selectedAnalytes)
                {
                    hiddenAnalytes.RemoveAll(item => item == Int32.Parse(analyte));
                }
                //remove site connected analytes
                foreach (var analyte in querySiteAnalytes)
                {
                    hiddenAnalytes.RemoveAll(item => item == analyte.analyteId);
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

            var bigLookupQuery =
                from datum in data
                join site in sites on new { siteId = datum.StationId } equals new { siteId = site.Id }
                join analyte in analytes on new { analyteId = datum.WaterQualityLabAnalyteId } equals new { analyteId = analyte.Id }
                //join standard in standards on new { analyteId = analyte.Id } equals new { analyteId = standard.WaterQualityLabAnalyteId }
                //join guideline in guidelines on new { guidelineId = standard.GuidelineId } equals new { guidelineId = guideline.Id }
                select new { 

                    siteId = datum.StationId,
                    siteName = site.SiteId,
                    siteType = site.SiteType,
                    
                    analyteId = analyte.Id, 
                    analyteName = analyte.AnalyteName,

                    //guidelineID = guideline.Id,
                    //guidelineName = guideline.GuidelineName
 
                };

            var returnSites = new List<Site>();
            var returnAnalytes = new List<Analyte>();
            var returnGuidelines = new List<Guideline>();

            var distinctSites =
                from row in bigLookupQuery
                group row by row.siteId
                    into sortedRows
                    select sortedRows.FirstOrDefault();

            foreach (var row in distinctSites)
            {
                var site = new Site { Id = row.siteId, SiteId = row.siteName, SiteType = row.siteType };
                returnSites.Add(site);
            }


            var distinctAnalytes =
                from row in bigLookupQuery
                group row by row.analyteId
                    into sortedRows
                    select sortedRows.FirstOrDefault();

            foreach (var row in distinctAnalytes)
            {
                var analyte = new Analyte { Id = row.analyteId,  AnalyteName = row.analyteName };
                returnAnalytes.Add(analyte);
            }

            //var distinctGuidelines =
            //    from row in bigLookupQuery
            //    group row by row.guidelineID
             //       into sortedRows
             //       select sortedRows.FirstOrDefault();

            //foreach (var row in distinctGuidelines)
            //{
           //     var guideline = new Guideline { Id = row.guidelineID, GuidelineName = row.guidelineName };
           //     returnGuidelines.Add(guideline);
           // }            
            
            string jsonResponse = Newtonsoft.Json.JsonConvert.SerializeObject(new { sites = returnSites, analytes = returnAnalytes, guidelines = guidelines });
            response.Content = new StringContent(jsonResponse);
            return response;
        }

        public class dateTimeRange
        {
            public string startDateTime { get; set; }
            public string endDateTime { get; set; }
        }

        [Route("WQ/DataAvailableDictionary")]
        [HttpGet]
        public HttpResponseMessage GetDataAvailableDictionary([FromUri] dateTimeRange[] dateRangeArray)
        {
            String queryStartDateTime = dateRangeArray[0].startDateTime;
            String queryEndDateTime = dateRangeArray[0].endDateTime;
            Debug.WriteLine(queryStartDateTime);
            Debug.WriteLine(queryEndDateTime);
            //RETURNS:
            // - Sites, Analytes, Guidelines
            // - 1D array of relations between Sites and Analytes with keys "siteId_analyteID"
            var response = new HttpResponseMessage();

            var SiteAnalyteLookup = new List<string>();

            //Load all data which will then be queried
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

            var bigLookupQuery =
                (from datum in data
                join site in sites on new { siteId = datum.StationId } equals new { siteId = site.Id }
                join analyte in analytes on new { analyteId = datum.WaterQualityLabAnalyteId } equals new { analyteId = analyte.Id }
                where datum.SampleDateTime >= Convert.ToDateTime(queryStartDateTime) && datum.SampleDateTime <= Convert.ToDateTime(queryEndDateTime)
                //join standard in standards on new { analyteId = analyte.Id } equals new { analyteId = standard.WaterQualityLabAnalyteId }
                //join guideline in guidelines on new { guidelineId = standard.GuidelineId } equals new { guidelineId = guideline.Id }
                select new
                {

                    siteId = datum.StationId,
                    siteName = site.SiteId,
                    siteType = site.SiteType,

                    analyteId = analyte.Id,
                    analyteName = analyte.AnalyteName,

                    //guidelineID = guideline.Id,
                    //guidelineName = guideline.GuidelineName

                }).Distinct();

            var yearsAvailable =
                (from datum in data
                select datum.SampleDateTime.Year).Distinct();

            foreach (var row in bigLookupQuery) {

                var key = row.siteId.ToString() + "_" + row.analyteId.ToString();
                SiteAnalyteLookup.Add(key);

            }

            string jsonResponse = Newtonsoft.Json.JsonConvert.SerializeObject(new { siteAnalyteLookupTable = SiteAnalyteLookup, sites = sites, analytes = analytes, guidelines = guidelines, yearsAvailable = yearsAvailable });
            response.Content = new StringContent(jsonResponse);
            return response;
        }
    }
}
