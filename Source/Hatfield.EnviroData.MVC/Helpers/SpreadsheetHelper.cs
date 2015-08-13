using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.SS.Util;

using Hatfield.EnviroData.Core;
using Hatfield.EnviroData.MVC.Models;

namespace Hatfield.EnviroData.MVC.Helpers
{
    public class SpreadsheetHelper
    {
        private static int DateRowIndex = 0;
        private static string ValueHeader = "Value";
        private static string PrefixHeader = "Prefix";
        private static string DlHeader = "Method Detection Limit";
        private static string NoValueContent = "-";


        public static IWorkbook GenerateQueryDataResultSpreadshet(string sheetName, IEnumerable<StationAnalyteQueryViewModel> viewModel, IEnumerable<Variable> analytes)
        {
            IWorkbook workbook = new XSSFWorkbook();
            var sheet = workbook.CreateSheet(sheetName);

            var distinctDates = viewModel.Select(x => x.ResultDateTime).Distinct();
            OutputDateRow(sheet, distinctDates, DateRowIndex);

            OutputDataRow(sheet, distinctDates, 2, viewModel, analytes);

            return workbook;
        }

        private static void OutputDateRow(ISheet sheet, IEnumerable<string> distinctDates, int rowIndex)
        {
            var colSpan = 3;
            var startCol = 1;

            var dateRow = sheet.CreateRow(rowIndex);
            var headerRow = sheet.CreateRow(rowIndex + 1);

            dateRow.CreateCell(0).SetCellValue("");
            headerRow.CreateCell(0).SetCellValue("");

            for (var i = 0; i < distinctDates.Count(); i++)
            {
                var cell = dateRow.CreateCell(startCol);
                cell.SetCellValue(distinctDates.ElementAt(i));
                sheet.AddMergedRegion(new CellRangeAddress(rowIndex, rowIndex, startCol, colSpan * (i + 1)));

                headerRow.CreateCell(startCol).SetCellValue(ValueHeader);
                headerRow.CreateCell(startCol + 1).SetCellValue(PrefixHeader);
                headerRow.CreateCell(startCol + 2).SetCellValue(DlHeader);

                startCol = startCol + colSpan;
            }

        }

        private static void OutputDataRow(ISheet sheet, 
                                          IEnumerable<string> distinctDates, 
                                          int rowIndex, 
                                          IEnumerable<StationAnalyteQueryViewModel> dataViewModels, 
                                          IEnumerable<Variable> analytes)
        {

            var dataViewModelsList = dataViewModels.ToList();

            //add selected analytes that does not have data
            foreach(var analyte in analytes)
            {
                if(dataViewModelsList.Where(x => x.Variable == analyte.VariableDefinition).FirstOrDefault() == null)
                {
                    dataViewModelsList.Add(new StationAnalyteQueryViewModel { 
                        Variable = analyte.VariableDefinition
                    });
                }
            }

            var analyteDataGroup = dataViewModelsList.GroupBy(x => x.Variable).OrderBy(x => x.Key);
            int columnIndex = 1;

            foreach(var analyteGroup in analyteDataGroup)
            {
                var dataRow = sheet.CreateRow(rowIndex);
                dataRow.CreateCell(0).SetCellValue(analyteGroup.Key);

                var analyteData = analyteGroup.ToList();

                foreach(var date in distinctDates)
                {
                    var matchedAnalyteData = analyteData.Where(x => x.ResultDateTime == date).FirstOrDefault();

                    if (matchedAnalyteData == null)
                    {
                        dataRow.CreateCell(columnIndex).SetCellValue(NoValueContent);
                        dataRow.CreateCell(columnIndex + 1).SetCellValue(NoValueContent);
                        dataRow.CreateCell(columnIndex + 2).SetCellValue(NoValueContent);
                    }
                    else
                    {
                        dataRow.CreateCell(columnIndex).SetCellValue(matchedAnalyteData.DataValue);
                        dataRow.CreateCell(columnIndex + 1).SetCellValue(matchedAnalyteData.Prefix);
                        dataRow.CreateCell(columnIndex + 2).SetCellValue(matchedAnalyteData.MethodDetectionLimit.HasValue ? matchedAnalyteData.MethodDetectionLimit.Value.ToString() : NoValueContent);
                    }

                    columnIndex = columnIndex + 3;
                }

                columnIndex = 1;

                rowIndex++;
            }


        }

    }
}