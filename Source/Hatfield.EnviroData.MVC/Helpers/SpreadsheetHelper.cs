using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;

using Hatfield.EnviroData.MVC.Models;

namespace Hatfield.EnviroData.MVC.Helpers
{
    public class SpreadsheetHelper
    {
        public static IWorkbook GenerateQueryDataResultSpreadshet(string sheetName, IEnumerable<StationAnalyteQueryViewModel> viewModel)
        {
            IWorkbook workbook = new XSSFWorkbook();
            var sheet = workbook.CreateSheet(sheetName);

            sheet.CreateRow(0).CreateCell(0).SetCellValue("Test Cell");

            return workbook;
        }
    }
}