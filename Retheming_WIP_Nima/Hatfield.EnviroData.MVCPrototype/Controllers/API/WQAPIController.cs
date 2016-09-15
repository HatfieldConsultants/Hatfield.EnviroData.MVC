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
    public class WQAPIController : ApiController
    {

        [Route("WQ/QueryData")]
        [HttpGet]
        public HttpResponseMessage Get()
        {
            var response = new HttpResponseMessage();
            String path = HttpContext.Current.Server.MapPath("~/assets/station.json");
            string text = System.IO.File.ReadAllText(path);
            //JObject stations = JObject.Parse(text);
            //string responseText = (string)stations[0];
            response.Content = new StringContent(text);
            return response;
        }

    }
}
