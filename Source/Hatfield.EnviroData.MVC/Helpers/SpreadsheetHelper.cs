using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using OfficeOpenXml;

namespace Hatfield.EnviroData.MVC.Helpers
{
    public class SpreadsheetHelper
    {
        public static ExcelPackage GenerateQueryDataResultSpreadshet(string sheetName)
        {
            var excelPackage = new ExcelPackage();
            var sheet = excelPackage.Workbook.Worksheets.Add(sheetName);

            sheet.Cells.Style.Font.Size = 11;
            sheet.Cells.Style.Font.Name = "Calibri";

            //1 based cell
            sheet.Cells[1, 1].Value = "Test Excel Data";
            return excelPackage;
        }
    }
}