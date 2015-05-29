using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

using Hatfield.EnviroData.Core;
using Hatfield.EnviroData.MVC.Models;
using Hatfield.EnviroData.DataAcquisition.ESDAT;

namespace Hatfield.EnviroData.MVC.Controllers.API
{
    public class QueryDataAPIController : ApiController
    {
        private readonly IActionRepository _actionRepository;

        public QueryDataAPIController(IActionRepository actionRepository)
        {
            _actionRepository = actionRepository;
        }

        [HttpGet]
        public IEnumerable<SampleListItemViewModel> Get()
        {
            //var allSamplingCollectionActions = _actionRepository.GetAllSampleCollectionActions();
            //var items = from domain in allSamplingCollectionActions
            //            select new SampleListItemViewModel(domain.ActionID, domain.BeginDateTime.ToString("mm-dd-yyyy"));

            var items = new List<SampleListItemViewModel> { 
                new SampleListItemViewModel(1, "test sample collection action"),
                new SampleListItemViewModel(2, "test sample collection action 2"),
            };
            return items.ToList();
        }

        [HttpGet]
        public ESDATModel GetSampleCollectionActionInESDAT(int Id)
        {
            //var matchedAction = _actionRepository.GetActionById(Id);

            var esdatModel = new ESDATModel(new DateTime(2014, 1, 1),
                                            1,
                                            "lab name",
                                            "lab signatory",
                                            new List<string>() { "associate file" },
                                            new List<string>() { "copy sent to" },
                                            2,
                                            3,
                                            4,
                                            5,
                                            (decimal)6.0,
                                            new List<SampleFileData>(),
                                            new List<ChemistryFileData>());

            return esdatModel;
        }
    }
}
