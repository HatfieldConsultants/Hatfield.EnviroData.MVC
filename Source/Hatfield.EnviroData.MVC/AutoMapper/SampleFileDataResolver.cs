﻿using System;
using System.Collections.Generic;
using System.Linq;

using AutoMapper;

using Hatfield.EnviroData.Core;
using Hatfield.EnviroData.DataAcquisition.ESDAT;
using Hatfield.EnviroData.DataAcquisition.ESDAT.Converters;

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
    }
}