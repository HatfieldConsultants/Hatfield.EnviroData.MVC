using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Hatfield.EnviroData.MVC.Models
{
    public class SiteViewModel
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public int SamplingFeatureID { get; set; }
        public string SiteTypeCV { get; set; }
        public int SpatialReferenceID { get; set; }
    }
}