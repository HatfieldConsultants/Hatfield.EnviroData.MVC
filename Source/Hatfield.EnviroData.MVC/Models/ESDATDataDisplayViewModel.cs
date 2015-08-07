using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Hatfield.EnviroData.DataAcquisition.ESDAT;

namespace Hatfield.EnviroData.MVC.Models
{
    public class ESDATDataDisplayViewModel
    {       
        //ESDAT data properties
        public List<string> AssociatedFiles { get; set; }
        public IEnumerable<ChemistryFileData> ChemistryData { get; set; }
        public int COCNumber { get; set; }
        public List<string> CopiesSentTo { get; set; }
        public DateTime DateReported { get; set; }
        public string LabName { get; set; }
        public int LabRequestId { get; set; }
        public int LabRequestNumber { get; set; }
        public decimal LabRequestVersion { get; set; }
        public string LabSignatory { get; set; }
        public int ProjectId { get; set; }
        public IEnumerable<SampleFileData> SampleFileData { get; set; }
        public int SDGID { get; set; }

        //versioning related data        
        public int CurrentSampleDataVersion { get; set; }
    }
}