using AutoMapper;
using Hatfield.EnviroData.Core;
using Hatfield.EnviroData.MVC.Models;
using Hatfield.EnviroData.WQDataProfile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Hatfield.EnviroData.MVC.Controllers.API
{
    public class StationQueryAPIController : ApiController
    {
        private readonly IActionRepository _actionRepository;
        private readonly ISiteRepository _siteRepository;
        private readonly IVariableRepository _variableRepository;
        private readonly IResultRepository _resultRepository;
        private readonly IMeasurementResultValueRepository _measurementResultValueRepository;
        private readonly IUnitRepository _unitRepository;

        public StationQueryAPIController(IActionRepository actionRepository, ISiteRepository siteRepository, IVariableRepository variableRepository, IResultRepository resultRepository, IMeasurementResultValueRepository measurementResultValueRepository, IUnitRepository unitRepository)
        {
            _actionRepository = actionRepository;
            _siteRepository = siteRepository;
            _variableRepository = variableRepository;
            _resultRepository = resultRepository;
            _measurementResultValueRepository = measurementResultValueRepository;
            _unitRepository = unitRepository;
        }

        [HttpGet]
        public IEnumerable<SiteViewModel> GetSites()
        {
            var sites = _siteRepository.GetAll();
            var items = Mapper.Map<IEnumerable<SiteViewModel>>(sites);
            return items;
        }

        [HttpGet]
        public SiteViewModel GetSingleSite(int locationId)
        {
            var sites = _siteRepository.GetAll().Where(x => x.SamplingFeatureID == locationId).FirstOrDefault();
            var item = Mapper.Map<SiteViewModel>(sites);
            return item;
        }

        //[HttpGet]
        //public IEnumerable<VariableViewModel> GetAllAnalytesForLocation(int locationId)
        //{
        //    var site = GetSingleSite(locationId);
        //    var analytes = 
        //    var items = Mapper.Map<IEnumerable<VariableViewModel>>(sites);
        //    return items;
        //}

        [HttpGet]
        public IEnumerable<VariableViewModel> GetAllAnalytes()
        {
            var sites = _variableRepository.GetAll();
            var items = Mapper.Map<IEnumerable<VariableViewModel>>(sites);
            return items;
        }

        [HttpPost]
        public IEnumerable<StationAnalyteQueryViewModel> FetchStationData(FetchSiteAnalyteQueryViewModel queryViewModel)
        {
            var items = new List<StationAnalyteQueryViewModel>();
            foreach (var variable in queryViewModel.SelectedVariables)
            {
                var results = _resultRepository.GetAll().Where(x => x.VariableID == variable && x.FeatureAction.SamplingFeatureID == queryViewModel.SelectedSiteID);
                if (results != null)
                {
                    foreach (var result in results)
                    {
                        if (result.MeasurementResult.MeasurementResultValues.First().ValueDateTime <= queryViewModel.EndDate && result.MeasurementResult.MeasurementResultValues.First().ValueDateTime >= queryViewModel.StartDate)
                        {
                            items.Add(new StationAnalyteQueryViewModel() { DataValue = result.MeasurementResult.MeasurementResultValues.First().DataValue, ResultDateTime = result.MeasurementResult.MeasurementResultValues.First().ValueDateTime, UnitsName = result.Unit.UnitsName, UnitsTypeCV = result.Unit.UnitsTypeCV, Variable = result.Variable.CV_VariableName.Name });
                        }
                    }
                }
            }

            return items;
        }
    }           
}
