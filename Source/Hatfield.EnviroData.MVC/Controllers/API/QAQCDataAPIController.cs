using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Text;

using Hatfield.EnviroData.Core;
using Hatfield.EnviroData.MVC.Models;
using Hatfield.EnviroData.QualityAssurance;
using Hatfield.EnviroData.WQDataProfile;
using Hatfield.EnviroData.QualityAssurance.DataFetchCriterias;
using Hatfield.EnviroData.QualityAssurance.DataQualityCheckingRules;
using Hatfield.EnviroData.QualityAssurance.DataQualityCheckingTool;

namespace Hatfield.EnviroData.MVC.Controllers.API
{
    public class QAQCDataAPIController : ApiController
    {
        private IActionRepository _actionRepository;
        private IRepository<CV_RelationshipType> _relatedActionTypeRepository;
        private IWQDefaultValueProvider _wqDefaultValueProvider;

        public QAQCDataAPIController(IActionRepository actionRepository, IRepository<CV_RelationshipType> relationTypeRepository, IWQDefaultValueProvider wqDefaultValueProvider)
        {
            _actionRepository = actionRepository;
            _relatedActionTypeRepository = relationTypeRepository;
            _wqDefaultValueProvider = wqDefaultValueProvider;
        }

        [HttpPost]
        [ActionName("QAQCChemistryData")]
        public ResultMessageViewModel QAQCChemistryData(IEnumerable<ChemistryQAQCDataEditViewModel> data)
        {
            var versioningHelper = new DataVersioningHelper(_wqDefaultValueProvider);
                        
            var items = from qaqcData in data
                        select new ChemistryValueCheckingRuleItem { 
                            ActionId = qaqcData.ActionId,
                            CorrectionValue = qaqcData.NewResultValue
                        };
            var rule = new ChemistryValueCheckingRule { 
                Items = items.ToList()
            };

            var dataFetchCriteria = new GetActionDataByIdsCriteria(_actionRepository, rule);

            var chemistryQAQCTool = new ChemistryValueCheckingTool(versioningHelper, _relatedActionTypeRepository);

            try
            { 
                //Check
                var qaqcResult = chemistryQAQCTool.Check(dataFetchCriteria.FetchData(), rule);

                //Correct
                //var correctionResult = chemistryQAQCTool.Correct(dataFetchCriteria.FetchData(), rule);

                if (qaqcResult.Level == QualityCheckingResultLevel.Info)
                {
                    var message = ConstructResultMessage(qaqcResult, "QA/QC data is saved.");
                    return new ResultMessageViewModel(ResultMessageViewModel.RESULT_LEVEL_INFO, message);
                }
                else
                {
                    var message = ConstructResultMessage(qaqcResult, "QA/QC data does not saved. Please check the result message for more detail.");
                    return new ResultMessageViewModel(ResultMessageViewModel.RESULT_LEVEL_FATAL, message);
                }
                
            }
            catch(Exception ex)
            {
                var message = "QA/QC data fail due to " + ex.StackTrace;
                return new ResultMessageViewModel(ResultMessageViewModel.RESULT_LEVEL_FATAL, message);
            }
            
        }

        private string ConstructResultMessage(IQualityCheckingResult result, string additionalMessage)
        {
            var messageBuilder = new StringBuilder();
            messageBuilder.Append(result.Message);
            messageBuilder.Append("<br/>");
            messageBuilder.Append(additionalMessage);

            return messageBuilder.ToString();
        }

    }
}
