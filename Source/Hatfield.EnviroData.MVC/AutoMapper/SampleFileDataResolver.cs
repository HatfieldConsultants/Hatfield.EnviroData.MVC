using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

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

                var sampleFileData = new SampleFileData{
                    SampledDateTime = samplingAction.BeginDateTime
                };

                result.Add(sampleFileData);
            }
            return result;
        }
    }
}