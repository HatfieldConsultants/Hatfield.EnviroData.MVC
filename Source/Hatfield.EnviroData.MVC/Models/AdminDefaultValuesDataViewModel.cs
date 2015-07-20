using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Hatfield.EnviroData.WQDataProfile;
namespace Hatfield.EnviroData.MVC.Models
{
    public class AdminDefaultValuesDataViewModel
    {
        public string ActionRelationshipTypeCVChemistry { get; set; }
        public string ActionRelationshipTypeCVSampleCollection { get; set; }
        public string ActionTypeCVChemistry { get; set; }
        public string ActionTypeCVSampleCollection { get; set; }
        public string DefaultDatasetTypeCV { get; set; }
        public string DefaultMethodTypeCVChemistry { get; set; }
        public string DefaultMethodTypeCVSampleCollection { get; set; }
        public string DefaultPersonFirstName { get; set; }
        public string DefaultPersonLastName { get; set; }
        public string DefaultPersonMiddleName { get; set; }

        public string DefaultOrganizationTypeCV { get; set; }
        public string DefaultOrganizationName { get; set; }
        public string DefaultOrganizationCode { get; set; }

        public string DefaultProcessingLevelCode { get; set; }
        public string DefaultSamplingFeatureCode { get; set; }
        public string DefaultSamplingFeatureTypeCVChemistry { get; set; }
        public string DefaultSamplingFeatureTypeCVSampleCollection { get; set; }
        public Guid DefaultSamplingFeatureUUID { get; set; }
        public string DefaultSRSCode { get; set; }
        public string DefaultSRSDescription { get; set; }
        public string DefaultSRSLink { get; set; }
        public string DefaultSRSName { get; set; }
        public string DefaultUnitsTypeCVChemistry { get; set; }
        public string DefaultUnitsTypeCVSampleCollection { get; set; }
        public string DefaultVariableCode { get; set; }
        public string DefaultVariableNameCV { get; set; }
        public double DefaultVariableNoDataValue { get; set; }
        public string DefaultVariableSpeciationCV { get; set; }
        public string DefaultVariableTypeCVChemistry { get; set; }
        public string DefaultVariableTypeCVSampleCollection { get; set; }
        public string MeasurementResultAggregationStatisticCVChemistry { get; set; }
        public string MeasurementResultCensorCodeCVChemistry { get; set; }
        public string MeasurementResultQualityCodeCVChemistry { get; set; }
        public string Name { get; set; }
        public string OrganizationNameSampleCollection { get; set; }
        public string OrganizationTypeCVChemistry { get; set; }
        public string OrganizationTypeCVSampleCollection { get; set; }
        public string ResultSampledMediumCVChemistry { get; set; }
        public string ResultSampledMediumCVSampleCollection { get; set; }
        public string ResultTypeCVChemistry { get; set; }
        public string ResultTypeCVSampleCollection { get; set; }
        public WayToHandleNewData WayToHandleNewData { get; set; }
    }
}