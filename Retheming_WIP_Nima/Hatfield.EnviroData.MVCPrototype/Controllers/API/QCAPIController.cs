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
    public class QCAPIController : ApiController
    {
        [Route("QC/ProvisionalDatasets")] //this route needs to be renamed, along with the other routes
        [HttpGet]
        public HttpResponseMessage Get()
        {
            var response = new HttpResponseMessage();
            string datasetsPath = HttpContext.Current.Server.MapPath("~/assets/sampleDatasets.json");
            string datasetsText = System.IO.File.ReadAllText(datasetsPath);
            string jsonResponse = Newtonsoft.Json.JsonConvert.SerializeObject(new { provisionalDatasets = datasetsText});
            response.Content = new StringContent(jsonResponse);
            return response;
        }

    }
}
