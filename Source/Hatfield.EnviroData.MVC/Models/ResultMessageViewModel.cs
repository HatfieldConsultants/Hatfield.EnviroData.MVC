using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Hatfield.EnviroData.MVC.Models
{
    public class ResultMessageViewModel
    {
        public static string RESULT_LEVEL_DEBUG = "DEBUG";
        public static string RESULT_LEVEL_INFO = "INFO";
        public static string RESULT_LEVEL_WARN = "WARN";
        public static string RESULT_LEVEL_ERROR = "ERROR";
        public static string RESULT_LEVEL_FATAL = "FATAL";

        public string Level { get; set; }//DEBUG, INFO, WARN, ERROR, FATAL
        public string Message { get; set; }

        public ResultMessageViewModel(string level, string message)
        {
            Level = level;
            Message = message;
        }
    }
}