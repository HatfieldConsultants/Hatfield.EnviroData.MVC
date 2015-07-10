using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Hatfield.EnviroData.MVC.Models
{
    public class ResultMessageViewModel
    {
        public string Level { get; set; }//DEBUG, INFO, WARN, ERROR, FATAL
        public string Message { get; set; }
    }
}