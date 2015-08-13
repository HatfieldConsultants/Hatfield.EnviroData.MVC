using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;

namespace Hatfield.EnviroData.MVC.Controllers
{
    public class FileDownloadController : Controller
    {
        [HttpGet]
        public FileResult DownloadQueryData(string fileName)
        {
            var relativePathPart = Path.Combine("App_Data", "Query_Data", fileName);
            var fileFullPath = Server.MapPath("~/" + relativePathPart);
            //var memoryStream = new MemoryStream();
            //var spreadSheet = SpreadsheetHelper.GenerateQueryDataResultSpreadshet("Results");

            //spreadSheet.Write(memoryStream);
            //var array = memoryStream.ToArray();

            return File(fileFullPath, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);

        }

    }
}
