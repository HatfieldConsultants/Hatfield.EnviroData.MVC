using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Hatfield.EnviroData.MVC.Models
{
    public class ChemistryQAQCDataEditViewModel
    {
        public int ActionId { get; set; }
        public double? OldResultValue { get; set; }
        public double? NewResultValue { get; set; }
    }
}