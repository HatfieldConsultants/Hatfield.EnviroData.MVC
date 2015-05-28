using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Hatfield.EnviroData.MVC.Models
{
    public class HttpFileImportDataViewModel
    {
        public string HeaderFileURL { get; set; }
        public string SampleFileURL { get; set; }
        public string ChemistryFileURL { get; set; }
    }
}