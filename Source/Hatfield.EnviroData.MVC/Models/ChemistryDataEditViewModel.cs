using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Hatfield.EnviroData.DataAcquisition.ESDAT;

namespace Hatfield.EnviroData.MVC.Models
{
    public class ChemistryDataEditViewModel
    {
        public int Id { get; set; }
        public ChemistryFileData ChemistryDataValue { get; set; }
    }
}