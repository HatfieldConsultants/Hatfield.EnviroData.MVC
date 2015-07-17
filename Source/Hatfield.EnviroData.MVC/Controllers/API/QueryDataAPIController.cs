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
using Hatfield.EnviroData.WQDataProfile;

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
            
        }
    }
}
