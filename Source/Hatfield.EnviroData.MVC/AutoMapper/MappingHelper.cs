using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Hatfield.EnviroData.MVC.AutoMapper
{
    public static class MappingHelper
    {
        public static double? ToNullableDouble(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return null;
            }
            else
            { 
                double parseValue = 0.0;
                if(double.TryParse(value, out parseValue))
                {
                    return parseValue;
                }
                else
                {
                    return null;
                }
            }
        }
    }
}