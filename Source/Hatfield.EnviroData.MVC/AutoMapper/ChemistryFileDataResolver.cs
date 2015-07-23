using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using AutoMapper;

using Hatfield.EnviroData.Core;
using Hatfield.EnviroData.DataAcquisition.ESDAT;
using Hatfield.EnviroData.DataAcquisition.ESDAT.Converters;

namespace Hatfield.EnviroData.MVC.AutoMapper
{
    public class ChemistryFileDataResolver : ValueResolver<IEnumerable<RelatedAction>, IEnumerable<ChemistryFileData>>
    {
        protected override IEnumerable<ChemistryFileData> ResolveCore(IEnumerable<RelatedAction> source)
        {
            var results = new List<ChemistryFileData>();

            foreach (var relationAction in source)
            {
                var featureActions = relationAction.Action1.FeatureActions;

                if(featureActions != null)
                {
                    foreach(var featureAction in featureActions)
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

        private ChemistryFileData MapChemistryFileData(Hatfield.EnviroData.Core.Action action, Result result, MeasurementResultValue measurementResultValue)
        {
            var chemistryFileData = new ChemistryFileData();

            chemistryFileData.ExtractionDate = action.BeginDateTime;
            chemistryFileData.AnalysedDate = action.EndDateTime.HasValue ? action.EndDateTime.Value : DateTime.MinValue;
            chemistryFileData.Result = measurementResultValue.DataValue;
            chemistryFileData.ResultUnit = result.MeasurementResult.Unit.UnitsName;
            chemistryFileData.OriginalChemName = result.Variable.VariableNameCV;
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