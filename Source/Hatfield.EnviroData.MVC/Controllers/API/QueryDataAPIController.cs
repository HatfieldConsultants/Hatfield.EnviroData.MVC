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
using Hatfield.EnviroData.MVC.Helpers;

namespace Hatfield.EnviroData.MVC.Controllers.API
{
    public class QueryDataAPIController : ApiController
    {
        private readonly IActionRepository _actionRepository;
        private readonly IWQDefaultValueProvider _wqDefaultValueProvider;

        public QueryDataAPIController(IActionRepository actionRepository, IWQDefaultValueProvider wqDefaultValueProvider)
        {
            _actionRepository = actionRepository;
            _wqDefaultValueProvider = wqDefaultValueProvider;
        }

        [HttpGet]
        public IEnumerable<SampleListItemViewModel> Get()
        {
            var allSamplingCollectionActions = _actionRepository.GetAllSampleCollectionActions();

            var items = Mapper.Map<IEnumerable<SampleListItemViewModel>>(allSamplingCollectionActions);
            return items.ToList();
        }

        [HttpGet]
        public ESDATModel GetSampleCollectionActionInESDAT(int Id, int? version = null)
        {
            var mappingHelper = new ESDATViewModelMappingHelper();
            var versionHelper = new DataVersioningHelper(_wqDefaultValueProvider);

            var matchedAction = _actionRepository.GetActionById(Id);

            var esdatModel = Mapper.Map<ESDATModel>(matchedAction);
            esdatModel.ChemistryData = ESDATViewModelMappingHelper.MapActionToChemistryFileData(matchedAction);

            if (version.HasValue)
            {
                if(version.Value == 0)
                {
                    esdatModel.SampleFileData = ESDATViewModelMappingHelper.MapActionToSampleFileData(matchedAction);
                }
                else if(version.Value > 0)
                {
                    while (version > 0)
                    {
                        var nextVersion = versionHelper.GetNextVersionActionData(matchedAction);
                        if (nextVersion == null)
                        {
                            throw new ArgumentException();
                        }
                        matchedAction = nextVersion;
                        version--;
                    }
                }
                else
                {
                    throw new ArgumentException();
                }
               

                esdatModel.SampleFileData = ESDATViewModelMappingHelper.MapActionToSampleFileData(matchedAction);
            }
            else
            {   
                esdatModel.SampleFileData = ESDATViewModelMappingHelper.MapActionToSampleFileData(matchedAction);
            }
            
            


            return esdatModel;
            
        }
    }
}
