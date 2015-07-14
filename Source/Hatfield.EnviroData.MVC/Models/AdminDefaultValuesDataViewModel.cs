using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Hatfield.EnviroData.WQDataProfile;
namespace Hatfield.EnviroData.MVC.Models
{
    public class AdminDefaultValuesDataViewModel
    {
        public string Name { get; set; }

        //Person Default Values
        public string DefaultPersonFirstName { get; set; }
        public string DefaultPersonMiddleName { get; set; }
        public string DefaultPersonLastName { get; set; }

        //Organization Default Values
        public string DefaultOrganizationTypeCV { get; set; }
        public string DefaultOrganizationName { get; set; }
        public string DefaultOrganizationCode { get; set; }

        //Processing Level Default Values
        public string DefaultProcessingLevels { get; set; }

        //Sampling Features Default Values
        public string DefaultSamplingFeatureUUID { get; set; }
        public string DefaultSamplingFeatureTypeCV { get; set; }
        public string DefaultSamplingFeatureCode { get; set; }

        //Method Default Values
        public string DefaultMethodTypeCV { get; set; }
        public string DefaultMethodCode { get; set; }
        public string DefaultMethodName { get; set; }
        public string DefaultMethodDescription { get; set; }
        public string DefaultMethodLink { get; set; }
        public string DefaultMethodOrganizationID { get; set; }

        //Variable Default Values
        public string DefaultVariableTypeCV { get; set; }
        public string DefaultVariableCode { get; set; }
        public string DefaultVariableNameCV { get; set; }
        public string DefaultVariableDefinition { get; set; }
        public string DefaultVariableSpeciationCV { get; set; }
        public double DefaultVariableNoDataValue { get; set; }

        //Units Default Values
        public string DefaultUnitsTypeCV { get; set; }
        public string DefaultUnitsAbbreviation { get; set; }
        public string DefaultUnitsName { get; set; }
        public string DefaultUnitsLink { get; set; }

        //CV Default Values
        public string DefaultCVUnitsType { get; set; }
        public string DefaultCVTerm { get; set; }
        public string DefaultCVName { get; set; }

        //Spatial Reference Default Values
        public string DefaultSRSCode { get; set; }
        public string DefaultSRSName { get; set; }
        public string DefaultSRSDescription { get; set; }
        public string DefaultSRSLink { get; set; }

        public WayToHandleNewData WayToHandleNewData { get; set; }
    }
}