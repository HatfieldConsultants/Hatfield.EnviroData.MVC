using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Hatfield.EnviroData.Core;
using Hatfield.EnviroData.WQDataProfile;
using Hatfield.EnviroData.DataAcquisition.ESDAT;
using Hatfield.EnviroData.DataAcquisition.ESDAT.Converters;
using Hatfield.EnviroData.MVC.AutoMapper;

namespace Hatfield.EnviroData.MVC.Helpers
{
    public class ESDATViewModelMappingHelper
    {
        public static IEnumerable<SampleFileData> MapActionToSampleFileData(Hatfield.EnviroData.Core.Action source)
        {
            var result = new List<SampleFileData> { };

            foreach (var featureAction in source.FeatureActions)
            {
                var samplingActionResults = featureAction.Results;

                if (samplingActionResults != null)
                {
                    foreach (var samplingActionResult in samplingActionResults)
                    {
                        var sampleFileData = MapSampleFileData(featureAction.Action, samplingActionResult);

                        if (sampleFileData != null)
                        {
                            result.Add(sampleFileData);
                        }
                    }

                }

            }
            return result;
        }

        public static IEnumerable<ChemistryFileData> MapActionToChemistryFileData(Hatfield.EnviroData.Core.Action source)
        {
            var relatedChemistryAction = source.RelatedActions.Where(x => x.CV_RelationshipType.Name == "Is related to");
            var results = new List<ChemistryFileData>();

            foreach (var relationAction in relatedChemistryAction)
            {
                var featureActions = relationAction.Action1.FeatureActions;

                if (featureActions != null)
                {
                    foreach (var featureAction in featureActions)
                    {
                        var measurementResults = from result in featureAction.Results
                                                 from measurementResultValue in result.MeasurementResult.MeasurementResultValues
                                                 select MapChemistryFileData(relationAction.Action1, result, measurementResultValue);

                        results.AddRange(measurementResults);
                    }
                }
            }

            return results;
        }

        private static SampleFileData MapSampleFileData(Hatfield.EnviroData.Core.Action actionData, Result sampleResultDomain)
        {
            if (sampleResultDomain != null)
            {

                var sampleFileData = new SampleFileData();
                sampleFileData.SampledDateTime = sampleResultDomain.ResultDateTime;

                try
                {
                    sampleFileData.LabName = actionData.Method.Organization.OrganizationName;
                }
                catch (NullReferenceException)
                {
                    sampleFileData.LabName = null;
                }

                //Map data from extension properties
                sampleFileData = MapFromExtensionProperties(sampleFileData, sampleResultDomain.ResultExtensionPropertyValues);

                return sampleFileData;


            }
            else
            {
                return null;
            }
        }

        private static SampleFileData MapFromExtensionProperties(SampleFileData sampleFileData, ICollection<ResultExtensionPropertyValue> extensionPropertyValues)
        {
            var propertyValueDictionary = extensionPropertyValues.ToDictionary(x => x.ExtensionProperty.PropertyName, x => x.PropertyValue);

            sampleFileData.SampleCode = propertyValueDictionary.ContainsKey(ESDATSampleCollectionConstants.ResultExtensionPropertyValueKeySampleCode) ?
                                            propertyValueDictionary[ESDATSampleCollectionConstants.ResultExtensionPropertyValueKeySampleCode] :
                                            string.Empty;

            sampleFileData.FieldID = propertyValueDictionary.ContainsKey(ESDATSampleCollectionConstants.ResultExtensionPropertyValueKeyFieldID) ?
                                        propertyValueDictionary[ESDATSampleCollectionConstants.ResultExtensionPropertyValueKeyFieldID] :
                                        string.Empty;

            sampleFileData.SampleDepth = propertyValueDictionary.ContainsKey(ESDATSampleCollectionConstants.ResultExtensionPropertyValueKeySampleDepth) ?
                                            MappingHelper.ToNullableDouble(propertyValueDictionary[ESDATSampleCollectionConstants.ResultExtensionPropertyValueKeySampleDepth]) :
                                            null;

            sampleFileData.MatrixType = propertyValueDictionary.ContainsKey(ESDATSampleCollectionConstants.ResultExtensionPropertyValueKeyMatrixType) ?
                                            propertyValueDictionary[ESDATSampleCollectionConstants.ResultExtensionPropertyValueKeyMatrixType] :
                                            string.Empty;

            sampleFileData.SampleType = propertyValueDictionary.ContainsKey(ESDATSampleCollectionConstants.ResultExtensionPropertyValueKeySampleType) ?
                                            propertyValueDictionary[ESDATSampleCollectionConstants.ResultExtensionPropertyValueKeySampleType] :
                                            string.Empty;

            sampleFileData.ParentSample = propertyValueDictionary.ContainsKey(ESDATSampleCollectionConstants.ResultExtensionPropertyValueKeyParentSample) ?
                                            propertyValueDictionary[ESDATSampleCollectionConstants.ResultExtensionPropertyValueKeyParentSample] :
                                            string.Empty;

            sampleFileData.SDG = propertyValueDictionary.ContainsKey(ESDATSampleCollectionConstants.ResultExtensionPropertyValueKeySDG) ?
                                            propertyValueDictionary[ESDATSampleCollectionConstants.ResultExtensionPropertyValueKeySDG] :
                                            string.Empty;

            sampleFileData.LabSampleID = propertyValueDictionary.ContainsKey(ESDATSampleCollectionConstants.ResultExtensionPropertyValueKeyLabSampleID) ?
                                            propertyValueDictionary[ESDATSampleCollectionConstants.ResultExtensionPropertyValueKeyLabSampleID] :
                                            string.Empty;

            sampleFileData.Comments = propertyValueDictionary.ContainsKey(ESDATSampleCollectionConstants.ResultExtensionPropertyValueKeyComments) ?
                                            propertyValueDictionary[ESDATSampleCollectionConstants.ResultExtensionPropertyValueKeyComments] :
                                            string.Empty;

            sampleFileData.LabReportNumber = propertyValueDictionary.ContainsKey(ESDATSampleCollectionConstants.ResultExtensionPropertyValueKeyLabReportNumber) ?
                                                propertyValueDictionary[ESDATSampleCollectionConstants.ResultExtensionPropertyValueKeyLabReportNumber] :
                                                string.Empty;

            return sampleFileData;
        }

        private static ChemistryFileData MapChemistryFileData(Hatfield.EnviroData.Core.Action action, Result result, MeasurementResultValue measurementResultValue)
        {
            var chemistryFileData = new ChemistryFileData();

            chemistryFileData.ExtractionDate = action.BeginDateTime;
            chemistryFileData.AnalysedDate = action.EndDateTime.HasValue ? action.EndDateTime.Value : DateTime.MinValue;
            chemistryFileData.Result = measurementResultValue.DataValue;
            chemistryFileData.ResultUnit = result.MeasurementResult.Unit.UnitsName;
            chemistryFileData.OriginalChemName = result.Variable.VariableDefinition;
            chemistryFileData.ChemCode = result.Variable.VariableCode;
            chemistryFileData.MethodName = action.Method.MethodName;
            chemistryFileData.MethodType = action.Method.MethodDescription;

            var propertyValueDictionary = result.ResultExtensionPropertyValues.ToDictionary(x => x.ExtensionProperty.PropertyName, x => x.PropertyValue);

            chemistryFileData.SampleCode = propertyValueDictionary.ContainsKey(ESDATChemistryConstants.ResultExtensionPropertyValueKeySampleCode) ?
                                            propertyValueDictionary[ESDATChemistryConstants.ResultExtensionPropertyValueKeySampleCode] :
                                            string.Empty;

            chemistryFileData.Prefix = propertyValueDictionary.ContainsKey(ESDATChemistryConstants.ResultExtensionPropertyValueKeyPrefix) ?
                                            propertyValueDictionary[ESDATChemistryConstants.ResultExtensionPropertyValueKeyPrefix] :
                                            string.Empty;

            chemistryFileData.TotalOrFiltered = propertyValueDictionary.ContainsKey(ESDATChemistryConstants.ResultExtensionPropertyValueKeyTotalOrFiltered) ?
                                                    propertyValueDictionary[ESDATChemistryConstants.ResultExtensionPropertyValueKeyTotalOrFiltered] :
                                                    string.Empty;

            chemistryFileData.ResultType = propertyValueDictionary.ContainsKey(ESDATChemistryConstants.ResultExtensionPropertyValueKeyResultType) ?
                                                propertyValueDictionary[ESDATChemistryConstants.ResultExtensionPropertyValueKeyResultType] :
                                                string.Empty;

            chemistryFileData.EQL = propertyValueDictionary.ContainsKey(ESDATChemistryConstants.ResultExtensionPropertyValueKeyEQL) ?
                                        MappingHelper.ToNullableDouble(propertyValueDictionary[ESDATChemistryConstants.ResultExtensionPropertyValueKeyEQL]) :
                                        null;

            chemistryFileData.EQLUnits = propertyValueDictionary.ContainsKey(ESDATChemistryConstants.ResultExtensionPropertyValueKeyEQLUnits) ?
                                            propertyValueDictionary[ESDATChemistryConstants.ResultExtensionPropertyValueKeyEQLUnits] :
                                            string.Empty;

            chemistryFileData.Comments = propertyValueDictionary.ContainsKey(ESDATChemistryConstants.ResultExtensionPropertyValueKeyComments) ? propertyValueDictionary[ESDATChemistryConstants.ResultExtensionPropertyValueKeyComments] : string.Empty;
            chemistryFileData.UCL = propertyValueDictionary.ContainsKey(ESDATChemistryConstants.ResultExtensionPropertyValueKeyUCL) ? MappingHelper.ToNullableDouble(propertyValueDictionary[ESDATChemistryConstants.ResultExtensionPropertyValueKeyUCL]) : null;
            chemistryFileData.LCL = propertyValueDictionary.ContainsKey(ESDATChemistryConstants.ResultExtensionPropertyValueKeyLCL) ? MappingHelper.ToNullableDouble(propertyValueDictionary[ESDATChemistryConstants.ResultExtensionPropertyValueKeyLCL]) : null;

            return chemistryFileData;
        }
    }
}