using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Hatfield.EnviroData.MVCPrototype.Controllers.API
{
    public class WQAPIController : ApiController
    {

        [Route("WQ/QueryData")]
        [HttpGet]
        public String Get()
        {
           
            return "Suh dude!";
        }

    }
}
