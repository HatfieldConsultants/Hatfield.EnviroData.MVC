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
        //private readonly IActionRepository _actionRepository;
        private readonly IWQDefaultValueProvider _wqDefaultValueProvider;
        private readonly IWQDataRepository _wqDataRepository;

        public QueryDataAPIController(IWQDataRepository wqDataRepository, IWQDefaultValueProvider wqDefaultValueProvider)
        {
            _wqDataRepository = wqDataRepository;
            _wqDefaultValueProvider = wqDefaultValueProvider;
        }

        [HttpGet]
        public IEnumerable<SampleListItemViewModel> Get()
        {
            var allSamplingCollectionActions = _wqDataRepository.GetAllWQSampleDataActions();

            var items = Mapper.Map<IEnumerable<SampleListItemViewModel>>(allSamplingCollectionActions);
            return items.ToList();
        }

        [HttpGet]        
        [ActionName("GetTotalSampleCollectionCount")]
        public int GetTotalSampleCollectionCount()
        {
            var allSamplingCollectionActions = _wqDataRepository.GetAllWQSampleDataActions();

            if(allSamplingCollectionActions == null || !allSamplingCollectionActions.Any())
            {
                return 0;
            }
            else
            {
                return allSamplingCollectionActions.Count();
            }
        }

        [HttpGet]
        [ActionName("GetTotalChemistryAnalyteCount")]
        public int GetTotalChemistryAnalyteCount()
        {
            var allSamplingCollectionActions = _wqDataRepository.GetAllWQAnalyteDataActions();

            if (allSamplingCollectionActions == null || !allSamplingCollectionActions.Any())
            {
                return 0;
            }
            else
            {
                return allSamplingCollectionActions.Count();
            }
        }

        [HttpGet]
        public ESDATDataDisplayViewModel GetSampleCollectionActionInESDAT(int Id, int? version = null)
        {
            var mappingHelper = new ESDATViewModelMappingHelper();
            var versionHelper = new DataVersioningHelper(_wqDefaultValueProvider);

            var matchedAction = _wqDataRepository.GetActionById(Id);

            var esdatModel = Mapper.Map<ESDATDataDisplayViewModel>(matchedAction);
            //no version function is applied to chemistry data yet
            esdatModel.ChemistryData = ESDATViewModelMappingHelper.MapActionToChemistryFileData(matchedAction, versionHelper);
            
            if (version.HasValue)
            {
                esdatModel.CurrentSampleDataVersion = version.Value;
                if(version.Value == 0)
                {
                    esdatModel.SampleFileData = ESDATViewModelMappingHelper.MapActionToSampleFileData(matchedAction);
                }
                else if(version.Value >= 1)
                {
                    while (version >= 1)
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
                //show the latest version data by default
                var numberOfSubversions = versionHelper.GetSubVersionCountOfAction(matchedAction);
                matchedAction = versionHelper.GetLatestVersionActionData(matchedAction);
                
                esdatModel.CurrentSampleDataVersion = numberOfSubversions;
                esdatModel.SampleFileData = ESDATViewModelMappingHelper.MapActionToSampleFileData(matchedAction);
            }

            return esdatModel;
            
        }

        [HttpGet]
        public IEnumerable<ChemistryDataEditViewModel> GetChemistryAnalyteDataBySampleActionId(int Id)
        {
            var mappingHelper = new ESDATViewModelMappingHelper();
            var versionHelper = new DataVersioningHelper(_wqDefaultValueProvider);

            var matchedAction = _wqDataRepository.GetActionById(Id);

            if (matchedAction != null)
            {
                var viewModels = ESDATViewModelMappingHelper.MapActionToChemistryFileEditViewModel(matchedAction, versionHelper);
                return viewModels;
            }
            else
            {
                return null;
            }
        }
    }
}
