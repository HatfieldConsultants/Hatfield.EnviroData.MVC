using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Hatfield.EnviroData.MVC.Models
{
    public class SampleListItemViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }

        //parameterless constructor for Automapper
        public SampleListItemViewModel()
        { 
        
        }

        public SampleListItemViewModel(int id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}