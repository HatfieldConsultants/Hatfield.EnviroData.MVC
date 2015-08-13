using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web;

using AutoMapper;
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;

using Hatfield.EnviroData.MVC.Helpers;
using Hatfield.EnviroData.MVC.Models;
using Hatfield.EnviroData.Core;
using Hatfield.EnviroData.WQDataProfile;
using System.IO;

namespace Hatfield.EnviroData.MVC.Controllers.API
{
    public class StationQueryAPIController : ApiController
    {
        private readonly IWQDefaultValueProvider _wqDefaultValueProvider;
        private readonly IWQDataRepository _wqDataRepository;
        private readonly ISiteRepository _siteRepository;
        private readonly IWQVariableRepository _variableRepository;        

        public StationQueryAPIController(IWQDefaultValueProvider wqDefaultValueProvider, ISiteRepository siteRepository, IWQVariableRepository variableRepository, IWQDataRepository wqDataRepository)
        {
            _wqDefaultValueProvider = wqDefaultValueProvider;
            _wqDataRepository = wqDataRepository;
            _siteRepository = siteRepository;
            _variableRepository = variableRepository;
        }

        [HttpGet]
        public IEnumerable<SiteViewModel> GetSites()
        {
            var sites = _siteRepository.GetAll();
            var items = Mapper.Map<IEnumerable<SiteViewModel>>(sites).OrderBy(x => x.SamplingFeatureName);
            return items;
        }

        [HttpGet]
        public SiteViewModel GetSingleSite(int locationId)
        {
            var sites = _siteRepository.GetAll().Where(x => x.SamplingFeatureID == locationId).FirstOrDefault();
            var item = Mapper.Map<SiteViewModel>(sites);
            return item;
        }

        [HttpGet]
        public IEnumerable<VariableViewModel> GetAllAnalytes()
        {
            var sites = _variableRepository.GetAllChemistryVariables().Where(x => x.VariableDefinition != null).OrderBy(x => x.VariableDefinition);
            var items = Mapper.Map<IEnumerable<VariableViewModel>>(sites);
            return items;
        }

        [HttpPost]
        public IEnumerable<StationAnalyteQueryViewModel> FetchStationData(FetchSiteAnalyteQueryViewModel queryViewModel)
        {
            var items = new List<StationAnalyteQueryViewModel>();
            var actions = _wqDataRepository.GetAllWQAnalyteDataActions();
            var versionHelper = new DataVersioningHelper(_wqDefaultValueProvider);

            if (queryViewModel.SelectedVariables != null && queryViewModel.SelectedSiteID != null)
            {
                foreach (var action in actions)
                {
                    foreach (var analyte in queryViewModel.SelectedVariables)
                    {
                        var latestAction = versionHelper.GetLatestVersionActionData(action);

                        var result = latestAction.FeatureActions.Where(x => x.SamplingFeatureID == queryViewModel.SelectedSiteID).FirstOrDefault().Results.Where(x => x.VariableID == analyte).FirstOrDefault();


                        if (result != null && result.ResultExtensionPropertyValues.Where(x => x.ExtensionProperty.PropertyName == "Result Type").FirstOrDefault().PropertyValue == "REG")
                        {
                            if (result.MeasurementResult != null && result.MeasurementResult.MeasurementResultValues.First().ValueDateTime <= queryViewModel.EndDate && result.MeasurementResult.MeasurementResultValues.First().ValueDateTime >= queryViewModel.StartDate)
                            {
                                var measurementValue = result.MeasurementResult.MeasurementResultValues.FirstOrDefault().DataValue;
                                var resultDateTime = latestAction.BeginDateTime;
                                var unitsName = result.Unit.UnitsName;
                                string prefix = null;
                                if (result.ResultExtensionPropertyValues.Where(x => x.ExtensionProperty.PropertyName == "Prefix").FirstOrDefault().PropertyValue != null)
                                {
                                    prefix = result.ResultExtensionPropertyValues.Where(x => x.ExtensionProperty.PropertyName == "Prefix").FirstOrDefault().PropertyValue;
                                }
                                var variable = result.Variable.VariableDefinition;
                                double? detectionLimit = null;
                                if (result.ResultsDataQualities.Count > 0)
                                {
                                    detectionLimit = result.ResultsDataQualities.Where(x => x.DataQuality.DataQualityTypeCV == "methodDetectionLimit").FirstOrDefault().DataQuality.DataQualityValue;
                                }
                                items.Add(new StationAnalyteQueryViewModel { DataValue = measurementValue, ResultDateTime = resultDateTime.ToString("MMM-dd-yyyy, HH:mm tt"), UnitsName = unitsName, Variable = variable, MethodDetectionLimit = detectionLimit, Prefix = prefix });
                            }
                        }
                    }
                }
            }
            return items;
        }

        [HttpPost]
        public string DownloadQueryData(FetchSiteAnalyteQueryViewModel queryViewModel)
        {
            var selectedAnalytes = _variableRepository.GetAllChemistryVariables()
                                              .Where(x => queryViewModel.SelectedVariables.Contains(x.VariableID))
                                              .AsEnumerable(); 

            var matchedSite = _siteRepository.GetAll().Where(x => x.SamplingFeatureID == queryViewModel.SelectedSiteID).FirstOrDefault();
            var siteName = (matchedSite == null || matchedSite.SamplingFeature == null || string.IsNullOrEmpty(matchedSite.SamplingFeature.SamplingFeatureName)) ? 
                            "Unknown" : 
                            matchedSite.SamplingFeature.SamplingFeatureName;

            var fileName = string.Format("{0}_Data_From_{1}_To_{2}.xlsx", siteName, queryViewModel.StartDate.ToString("MMM-dd-yyyy"), queryViewModel.EndDate.ToString("MMM-dd-yyyy"));

            var relativePathPart = Path.Combine("App_Data", "Query_Data", fileName);
            var fileFullPath = HttpContext.Current.Server.MapPath("~/" + relativePathPart);

            var dataViewModel = FetchStationData(queryViewModel);
            var spreadSheet = SpreadsheetHelper.GenerateQueryDataResultSpreadshet("Results", dataViewModel, selectedAnalytes);

            using(var fileStream = System.IO.File.Create(fileFullPath))
            {                
                spreadSheet.Write(fileStream);
            }
            return fileName;

        }
    }
}
