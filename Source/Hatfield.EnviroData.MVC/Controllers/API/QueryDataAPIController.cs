using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

using AutoMapper;

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
            var allSamplingCollectionActions = _actionRepository.GetAllSampleCollectionActions();

            var items = Mapper.Map<IEnumerable<SampleListItemViewModel>>(allSamplingCollectionActions);
            return items.ToList();
        }

        [HttpGet]
        public ESDATModel GetSampleCollectionActionInESDAT(int Id)
        {
            var matchedAction = _actionRepository.GetActionById(Id);

            var esdatModel = Mapper.Map<ESDATModel>(matchedAction);
            return esdatModel;
            //var esdatModel = new ESDATModel(new DateTime(2014, 1, 1),
            //                                1,
            //                                "lab name",
            //                                "lab signatory",
            //                                new List<string>() { "associate file" },
            //                                new List<string>() { "copy sent to" },
            //                                2,
            //                                3,
            //                                4,
            //                                5,
            //                                (decimal)6.0,
            //                                new List<SampleFileData> { 
            //                                    new SampleFileData("test sample code",
            //                                                        new DateTime(2015, 1, 5),
            //                                                        "test field ID",
            //                                                        1.0,
            //                                                        "Water",
            //                                                        "test sample type",
            //                                                        "",
            //                                                        "test SDG",
            //                                                        "test lab Name",
            //                                                        "test lab sample ID",
            //                                                        "test comment",
            //                                                        "test lab report number")
            //                                },
            //                                new List<ChemistryFileData> { 
            //                                    new ChemistryFileData("test sample code", "test orginial chem name", "chem code", "<", 2.0,
            //                                                         "%", "Total", "result type", "method type", "method name", new DateTime(2015, 1, 6),
            //                                                         new DateTime(2015, 1, 6), 3.0, "test EQL units", "test comments", "Good", 4.0, 5.0)
            //                                });

            
        }
    }
}
