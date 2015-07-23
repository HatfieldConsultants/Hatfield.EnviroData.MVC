using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Hatfield.EnviroData.MVC.Models
{
    public class FetchSiteAnalyteQueryViewModel
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int SelectedSiteID { get; set; }
        public IEnumerable<int> SelectedVariables { get; set; }
    }
}