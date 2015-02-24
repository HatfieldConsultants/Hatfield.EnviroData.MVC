using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

using Hatfield.EnviroData.Core;

namespace Hatfield.EnviroData.MVC.Controllers
{
    public class ValuesController : ApiController
    {
        private IVariableRepository _variableRepository;

        public ValuesController(IVariableRepository variableRepository)
        {
            _variableRepository = variableRepository;
        }

        // GET api/values
        public IEnumerable<string> Get()
        {
            var allVariables = _variableRepository.GetAll();

            return allVariables.Select(x => x.VariableNameCV);
        }

    }
}