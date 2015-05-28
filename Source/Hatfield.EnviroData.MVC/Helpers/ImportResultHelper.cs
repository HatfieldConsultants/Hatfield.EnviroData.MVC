using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Hatfield.EnviroData.DataAcquisition;

namespace Hatfield.EnviroData.MVC.Helpers
{
    public class ImportResultHelper
    {
        public static IEnumerable<IResult> FilterWarningAndErrorResult(IEnumerable<IResult> results)
        {
            var filteredResults = results.Where(x => ResultLevelHelper.LevelIsHigherThanOrEqualToThreshold(ResultLevel.WARN, x.Level));

            return filteredResults;
        }
    }
}