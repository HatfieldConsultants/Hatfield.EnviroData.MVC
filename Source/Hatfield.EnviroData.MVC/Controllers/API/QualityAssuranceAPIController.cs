using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

using AutoMapper;

using Hatfield.EnviroData.Core;
using Hatfield.EnviroData.MVC.Models;
using Hatfield.EnviroData.QualityAssurance;
using Hatfield.EnviroData.WQDataProfile;
using Hatfield.EnviroData.QualityAssurance.DataFetchCriterias;
using Hatfield.EnviroData.QualityAssurance.DataQualityCheckingRules;
using Hatfield.EnviroData.QualityAssurance.DataQualityCheckingTool;

namespace Hatfield.EnviroData.MVC.Controllers.API
{
    public class QualityAssuranceAPIController : ApiController
    {
        private IWQDataRepository _wqDataRepository;
        private IRepository<CV_RelationshipType> _relatedActionTypeRepository;
        private IWQDefaultValueProvider _wqDefaultValueProvider;
        
        public QualityAssuranceAPIController(IWQDataRepository wqDataRepository, IRepository<CV_RelationshipType> relationTypeRepository, IWQDefaultValueProvider wqDefaultValueProvider)
        {
            _wqDataRepository = wqDataRepository;
            _relatedActionTypeRepository = relationTypeRepository;
            _wqDefaultValueProvider = wqDefaultValueProvider;
        }

        [HttpPost]
        [ActionName("ExecuteQualityAssuranceChain")]
        public IEnumerable<ResultMessageViewModel> ExecuteQualityAssuranceChain(QualityCheckingExecuteDataViewModel data)
        {
            var resultMessages = new List<ResultMessageViewModel>();

            if (data != null)
            {
                var qcChainConfiguration = MapTOChainConfiguration(data);

                var versioningHelper = new DataVersioningHelper(_wqDefaultValueProvider);
                var factory = new DataQualityCheckingToolFactory(versioningHelper, _relatedActionTypeRepository);
                var qualiltyChecker = new WaterQualityDataQualityChecker(qcChainConfiguration, factory, _wqDataRepository);

                var qcResults = qualiltyChecker.Check();

                var resultViewModels = Mapper.Map<IEnumerable<IQualityCheckingResult>, IEnumerable<ResultMessageViewModel>>(qcResults);

                resultMessages.AddRange(resultViewModels);
            }
            else
            {
                resultMessages.Add(new ResultMessageViewModel(ResultMessageViewModel.RESULT_LEVEL_FATAL, 
                                                              "QC request data is null. No QC process could be applied."));
            }

            return resultMessages;
            
        }

        private DataQualityCheckingChainConfiguration MapTOChainConfiguration(QualityCheckingExecuteDataViewModel data)
        {
            var chainConfiguration = new DataQualityCheckingChainConfiguration();

            chainConfiguration.NeedToCorrectData = data.NeedCorrection;
            chainConfiguration.DataFetchCriteria = new GetAllWaterQualitySampleDataCriteria(_wqDataRepository);

            var sampleMatrixCheckingRuleConfiguration = new DataQualityCheckingToolConfiguration();
            sampleMatrixCheckingRuleConfiguration.DataQualityCheckingToolType = typeof(SampleMatrixTypeCheckingTool);
            sampleMatrixCheckingRuleConfiguration.DataQualityCheckingRule = new StringCompareCheckingRule(data.SampleMatrixTypeCheckingToolExpectedValue,
                                                                                             data.SampleMatrixTypeCheckingToolCaseSensitive,
                                                                                             data.SampleMatrixTypeCheckingToolCorrectionValue
                                                                                            );

            chainConfiguration.ToolsConfiguration = new List<DataQualityCheckingToolConfiguration> { sampleMatrixCheckingRuleConfiguration };

            return chainConfiguration;
        }
    }
}
