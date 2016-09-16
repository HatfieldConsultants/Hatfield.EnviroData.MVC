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
    public class HomeAPIController : ApiController
    {
        //This controller will instead call WQAPIController, QCAPIController, etc. for required data. 
        //NOTE: Is this controller even necessary? I can do without this and just have script.js recognize the home page and make the required calls to QCAPI/WQAPI/etc...
        [Route("Home/InfoRefresh")] //this route needs to be renamed, along with the other routes
        [HttpGet]
        public HttpResponseMessage Get()
        {
            var response = new HttpResponseMessage();
            string sampleDatasetsPath = HttpContext.Current.Server.MapPath("~/assets/sampleDatasets.json");
            string sampleDatasetsText = System.IO.File.ReadAllText(sampleDatasetsPath); 
            string jsonResponse = Newtonsoft.Json.JsonConvert.SerializeObject(new { provisionalDatasets = sampleDatasetsText, recentDatasets = sampleDatasetsText });
            response.Content = new StringContent(jsonResponse);
            return response;
        }

    }
}
