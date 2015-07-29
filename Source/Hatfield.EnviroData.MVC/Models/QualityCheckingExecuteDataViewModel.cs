using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Hatfield.EnviroData.MVC.Models
{
    public class QualityCheckingExecuteDataViewModel
    {
        public string ChainName { get; set; }
        public string DataFetchingCriteriaName { get; set; }
        public bool NeedCorrection { get; set; }

        public string SampleMatrixTypeCheckingToolExpectedValue { get; set; }
        public string SampleMatrixTypeCheckingToolCorrectionValue { get; set; }
        public bool SampleMatrixTypeCheckingToolCaseSensitive { get; set; }
    }
}