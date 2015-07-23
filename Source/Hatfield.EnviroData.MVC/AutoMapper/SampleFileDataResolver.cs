using System;
using System.Collections.Generic;
using System.Linq;

using AutoMapper;

using Hatfield.EnviroData.Core;
using Hatfield.EnviroData.DataAcquisition.ESDAT;

namespace Hatfield.EnviroData.MVC.AutoMapper
{
    public class SampleFileDataResolver : ValueResolver<IEnumerable<RelatedAction>, IEnumerable<SampleFileData>>
    {
        protected override IEnumerable<SampleFileData> ResolveCore(IEnumerable<RelatedAction> source)
        {
            var result = new List<SampleFileData>{};

            foreach(var relationAction in source)
            {
                var samplingAction  = relationAction.Action1;

                var sampleFileData = MapSampleFileDataFromAction(samplingAction);

                if(sampleFileData != null)
                {
                    result.Add(sampleFileData);
                }                
            }
            return result;
        }

        private SampleFileData MapSampleFileDataFromAction(Hatfield.EnviroData.Core.Action actionData)
        {
            var sampleResultsDomain = actionData.FeatureActions.Select(x => x.Results).FirstOrDefault();

            if (sampleResultsDomain != null)
            {
                var sampleResultDomain = sampleResultsDomain.FirstOrDefault();

                if(sampleResultDomain != null)
                {
                    var sampleFileData = new SampleFileData();
                    sampleFileData.SampledDateTime = sampleResultDomain.ResultDateTime;

                    try
                    {
                        sampleFileData.LabName = actionData.Method.Organization.OrganizationName;
                    }
                    catch(NullReferenceException)
                    {
                        sampleFileData.LabName = null;
                    }
                    
                    //Map data from extension properties
                    sampleFileData = MapFromExtensionProperties(sampleFileData, sampleResultDomain.ResultExtensionPropertyValues);

                    return sampleFileData;
                    
                }

                return null;
                
            }
            else
            {
                return null;
            }
        }

        private SampleFileData MapFromExtensionProperties(SampleFileData sampleFileData, ICollection<ResultExtensionPropertyValue> extensionPropertyValues)
        {
            var propertyValueDictionary = extensionPropertyValues.ToDictionary(x => x.ExtensionProperty.PropertyName, x => x.PropertyValue);

            sampleFileData.SampleCode = propertyValueDictionary.ContainsKey("SampleCode") ? propertyValueDictionary["SampleCode"] : string.Empty;
            sampleFileData.FieldID = propertyValueDictionary.ContainsKey("Field ID") ? propertyValueDictionary["Field ID"] : string.Empty;
            sampleFileData.SampleDepth = propertyValueDictionary.ContainsKey("Sample Depth") ? MappingHelper.ToNullableDouble(propertyValueDictionary["Sample Depth"]) : null;
            sampleFileData.MatrixType = propertyValueDictionary.ContainsKey("Matrix Type") ? propertyValueDictionary["Matrix Type"] : string.Empty;
            sampleFileData.SampleType = propertyValueDictionary.ContainsKey("Sample Type") ? propertyValueDictionary["Sample Type"] : string.Empty;
            sampleFileData.ParentSample = propertyValueDictionary.ContainsKey("Parent Sample") ? propertyValueDictionary["Parent Sample"] : string.Empty;
            sampleFileData.SDG = propertyValueDictionary.ContainsKey("SDG") ? propertyValueDictionary["SDG"] : string.Empty;
            sampleFileData.LabSampleID = propertyValueDictionary.ContainsKey("Lab SampleID") ? propertyValueDictionary["Lab SampleID"] : string.Empty;
            sampleFileData.Comments = propertyValueDictionary.ContainsKey("Comments") ? propertyValueDictionary["Comments"] : string.Empty;
            sampleFileData.LabReportNumber = propertyValueDictionary.ContainsKey("Lab Report Number") ? propertyValueDictionary["Lab Report Number"] : string.Empty;

            return sampleFileData;
        }
    }
}