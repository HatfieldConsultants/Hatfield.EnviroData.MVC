using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

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
        private IWQDataRepository _wqDataRepository;
        private IRepository<CV_RelationshipType> _relatedActionTypeRepository;
        private IWQDefaultValueProvider _wqDefaultValueProvider;

        public QAQCDataAPIController(IWQDataRepository wqDataRepository, IRepository<CV_RelationshipType> relationTypeRepository, IWQDefaultValueProvider wqDefaultValueProvider)
        {
            _wqDataRepository = wqDataRepository;
            _relatedActionTypeRepository = relationTypeRepository;
            _wqDefaultValueProvider = wqDefaultValueProvider;
        }

        [HttpPost]
        [ActionName("QAQCChemistryData")]
        public ResultMessageViewModel QAQCChemistryData(IEnumerable<ChemistryQAQCDataEditViewModel> data)
        {
            return new ResultMessageViewModel(ResultMessageViewModel.RESULT_LEVEL_INFO, "QAQC data is saved.");
        }

    }
}
