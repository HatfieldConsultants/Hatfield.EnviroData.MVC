using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using AutoMapper;

using Hatfield.EnviroData.Core;
using Hatfield.EnviroData.DataAcquisition.ESDAT;

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

            chemistryFileData.SampleCode = propertyValueDictionary.ContainsKey("SampleCode") ? propertyValueDictionary["SampleCode"] : string.Empty;
            chemistryFileData.Prefix = propertyValueDictionary.ContainsKey("Prefix") ? propertyValueDictionary["Prefix"] : string.Empty;
            chemistryFileData.TotalOrFiltered = propertyValueDictionary.ContainsKey("Total or Filtered") ? propertyValueDictionary["Total or Filtered"] : string.Empty;
            chemistryFileData.ResultType = propertyValueDictionary.ContainsKey("Result Type") ? propertyValueDictionary["Result Type"] : string.Empty;
            chemistryFileData.EQL = propertyValueDictionary.ContainsKey("EQL") ? MappingHelper.ToNullableDouble(propertyValueDictionary["EQL"]) : null;
            chemistryFileData.EQLUnits = propertyValueDictionary.ContainsKey("EQL Units") ? propertyValueDictionary["EQL Units"] : string.Empty;
            chemistryFileData.Comments = propertyValueDictionary.ContainsKey("Comments") ? propertyValueDictionary["Comments"] : string.Empty;
            chemistryFileData.UCL = propertyValueDictionary.ContainsKey("UCL") ? MappingHelper.ToNullableDouble(propertyValueDictionary["UCL"]) : null;
            chemistryFileData.LCL = propertyValueDictionary.ContainsKey("LCL") ? MappingHelper.ToNullableDouble(propertyValueDictionary["LCL"]) : null;

            return chemistryFileData;
        }
    }
}