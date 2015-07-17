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
                                                 select new ChemistryFileData {
                                                     AnalysedDate = measurementResultValue.ValueDateTime,
                                                     ExtractionDate = measurementResultValue.ValueDateTime,
                                                     Result = measurementResultValue.DataValue,
                                                     ResultUnit = measurementResultValue.MeasurementResult.Unit.UnitsName,
                                                     OriginalChemName = result.Variable.VariableNameCV
                                                     
                                                     
                                                    
                                                 };

                        results.AddRange(measurementResults);
                    }
                }
            }

            return results;
        }
    }
}