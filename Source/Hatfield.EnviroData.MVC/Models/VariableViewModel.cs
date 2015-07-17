using Hatfield.EnviroData.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Hatfield.EnviroData.MVC.Models
{
    public class VariableViewModel
    {
        public double NoDataValue { get; set; }
        public string SpeciationCV { get; set; }
        public string VariableCode { get; set; }
        public string VariableDefinition { get; set; }
        public int VariableID { get; set; }
        public string VariableNameCV { get; set; }
        public string VariableTypeCV { get; set; }
    }
}