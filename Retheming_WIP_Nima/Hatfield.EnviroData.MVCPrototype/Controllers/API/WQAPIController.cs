using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web;

namespace Hatfield.EnviroData.MVCPrototype.Controllers.API
{
    public class WQAPIController : ApiController
    {

        [Route("WQ/QueryData")]
        [HttpGet]
        public string Get()
        {
            String path = HttpContext.Current.Server.MapPath("~/assets/station.json");
            string text = System.IO.File.ReadAllText(path);
            return text;
        }

    }
}
