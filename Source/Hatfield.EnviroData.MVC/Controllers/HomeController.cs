using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Hatfield.EnviroData.Core;

namespace Hatfield.EnviroData.MVC.Controllers
{
    public class HomeController : Controller
    {
        private IVariableRepository _variableRepository;

        public HomeController(IVariableRepository variableRepository)
        {
            _variableRepository = variableRepository;
        }

        public ActionResult Index()
        {
            var allVariables = _variableRepository.GetAll();
            return View(allVariables);
        }
    }
}
