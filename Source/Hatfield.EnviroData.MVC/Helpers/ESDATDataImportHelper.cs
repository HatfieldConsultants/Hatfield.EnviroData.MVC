using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Hatfield.EnviroData.DataAcquisition.Criterias;
using Hatfield.EnviroData.DataAcquisition.ValueAssigners;
using Hatfield.EnviroData.DataAcquisition.CSV.Importers;
using Hatfield.EnviroData.DataAcquisition.CSV.ValidationRules;
using Hatfield.EnviroData.DataAcquisition.ESDAT;
using Hatfield.EnviroData.DataAcquisition.CSV;
using Hatfield.EnviroData.FileSystems.WindowsFileSystem;
using Hatfield.EnviroData.DataAcquisition.ESDAT.Importer;
using Hatfield.EnviroData.DataAcquisition.XML;
using Hatfield.EnviroData.DataAcquisition;
using Hatfield.EnviroData.WQDataProfile;

namespace Hatfield.EnviroData.MVC.Helpers
{
    public class ESDATDataImportHelper
    {
        public static IDataImporter BuildESDATDataImporter(IWQDefaultValueProvider wqDefaultValueProvider)
        {
            var simpleValueAssginer = new SimpleValueAssigner();

            var sampleDataImporter = BuildSampleDataFileImporter();
            var sampleFileChildObjectExtractConfiguration = new SampleFileChildObjectExtractConfiguration(sampleDataImporter, "SampleFileData", simpleValueAssginer);

            var chemistryDataImporter = BuildChemistryFileImporter();
            var chemistryFileChildObjectExtractConfiguration = new ChemistryFileChildObjectExtractConfiguration(chemistryDataImporter, "ChemistryData", simpleValueAssginer);

            var ESDATDataImporter = new ESDATDataImporter(ResultLevel.ERROR, wqDefaultValueProvider);

            AddXMLExtractConfigurationsToImporter(ESDATDataImporter);
            ESDATDataImporter.AddExtractConfiguration(sampleFileChildObjectExtractConfiguration);
            ESDATDataImporter.AddExtractConfiguration(chemistryFileChildObjectExtractConfiguration);

            return ESDATDataImporter;
        }

        private static IDataImporter BuildChemistryFileImporter()
        {
            var simpleValueAssigner = new SimpleValueAssigner();

            var parserFactory = new DefaultCSVParserFactory();

            var testImporter = new SimpleCSVDataImporter(ResultLevel.ERROR, 1);

            testImporter.AddExtractConfiguration(new SimpleCSVExtractConfiguration(0,
                                                                                    "SampleCode",
                                                                                    parserFactory.GetCellParser(typeof(string)),
                                                                                    simpleValueAssigner,
                                                                                    typeof(string)));

            testImporter.AddExtractConfiguration(new SimpleCSVExtractConfiguration(1,
                                                                                    "ChemCode",
                                                                                    parserFactory.GetCellParser(typeof(string)),
                                                                                    simpleValueAssigner,
                                                                                    typeof(string)));

            testImporter.AddExtractConfiguration(new SimpleCSVExtractConfiguration(2,
                                                                                    "OriginalChemName",
                                                                                    parserFactory.GetCellParser(typeof(string)),
                                                                                    simpleValueAssigner,
                                                                                    typeof(string)));

            testImporter.AddExtractConfiguration(new SimpleCSVExtractConfiguration(3,
                                                                                    "Prefix",
                                                                                    parserFactory.GetCellParser(typeof(string)),
                                                                                    simpleValueAssigner,
                                                                                    typeof(string)));

            testImporter.AddExtractConfiguration(new SimpleCSVExtractConfiguration(4,
                                                                                    "Result",
                                                                                    parserFactory.GetCellParser(typeof(double?)),
                                                                                    simpleValueAssigner,
                                                                                    typeof(double?)));

            testImporter.AddExtractConfiguration(new SimpleCSVExtractConfiguration(5,
                                                                                    "ResultUnit",
                                                                                    parserFactory.GetCellParser(typeof(string)),
                                                                                    simpleValueAssigner,
                                                                                    typeof(string)));

            testImporter.AddExtractConfiguration(new SimpleCSVExtractConfiguration(6,
                                                                                    "TotalOrFiltered",
                                                                                    parserFactory.GetCellParser(typeof(string)),
                                                                                    simpleValueAssigner,
                                                                                    typeof(string)));

            testImporter.AddExtractConfiguration(new SimpleCSVExtractConfiguration(7,
                                                                                    "ResultType",
                                                                                    parserFactory.GetCellParser(typeof(string)),
                                                                                    simpleValueAssigner,
                                                                                    typeof(string)));

            testImporter.AddExtractConfiguration(new SimpleCSVExtractConfiguration(8,
                                                                                    "MethodType",
                                                                                    parserFactory.GetCellParser(typeof(string)),
                                                                                    simpleValueAssigner,
                                                                                    typeof(string)));

            testImporter.AddExtractConfiguration(new SimpleCSVExtractConfiguration(9,
                                                                                    "MethodName",
                                                                                    parserFactory.GetCellParser(typeof(string)),
                                                                                    simpleValueAssigner,
                                                                                    typeof(string)));

            testImporter.AddExtractConfiguration(new SimpleCSVExtractConfiguration(10,
                                                                                    "ExtractionDate",
                                                                                    parserFactory.GetCellParser(typeof(DateTime?)),
                                                                                    simpleValueAssigner,
                                                                                    typeof(DateTime?)));

            testImporter.AddExtractConfiguration(new SimpleCSVExtractConfiguration(11,
                                                                                    "AnalysedDate",
                                                                                    parserFactory.GetCellParser(typeof(DateTime?)),
                                                                                    simpleValueAssigner,
                                                                                    typeof(DateTime?)));

            testImporter.AddExtractConfiguration(new SimpleCSVExtractConfiguration(12,
                                                                                    "EQL",
                                                                                    parserFactory.GetCellParser(typeof(double?)),
                                                                                    simpleValueAssigner,
                                                                                    typeof(double?)));

            testImporter.AddExtractConfiguration(new SimpleCSVExtractConfiguration(13,
                                                                                    "EQLUnits",
                                                                                    parserFactory.GetCellParser(typeof(string)),
                                                                                    simpleValueAssigner,
                                                                                    typeof(string)));

            testImporter.AddExtractConfiguration(new SimpleCSVExtractConfiguration(14,
                                                                                    "Comments",
                                                                                    parserFactory.GetCellParser(typeof(string)),
                                                                                    simpleValueAssigner,
                                                                                    typeof(string)));

            testImporter.AddExtractConfiguration(new SimpleCSVExtractConfiguration(16,
                                                                                    "UCL",
                                                                                    parserFactory.GetCellParser(typeof(double?)),
                                                                                    simpleValueAssigner,
                                                                                    typeof(double?)));

            testImporter.AddExtractConfiguration(new SimpleCSVExtractConfiguration(17,
                                                                                    "LCL",
                                                                                    parserFactory.GetCellParser(typeof(double?)),
                                                                                    simpleValueAssigner,
                                                                                    typeof(double?)));
            return testImporter;

        }

        private static IDataImporter BuildSampleDataFileImporter()
        {
            var simpleValueAssigner = new SimpleValueAssigner();

            var parserFactory = new DefaultCSVParserFactory();

            var testImporter = new SimpleCSVDataImporter(ResultLevel.ERROR, 1);

            testImporter.AddExtractConfiguration(new SimpleCSVExtractConfiguration(0,
                                                                                    "SampleCode",
                                                                                    parserFactory.GetCellParser(typeof(string)),
                                                                                    simpleValueAssigner,
                                                                                    typeof(string)));

            testImporter.AddExtractConfiguration(new SimpleCSVExtractConfiguration(1,
                                                                                    "SampledDateTime",
                                                                                    parserFactory.GetCellParser(typeof(DateTime?)),
                                                                                    simpleValueAssigner,
                                                                                    typeof(DateTime?)));

            testImporter.AddExtractConfiguration(new SimpleCSVExtractConfiguration(2,
                                                                                    "FieldID",
                                                                                    parserFactory.GetCellParser(typeof(string)),
                                                                                    simpleValueAssigner,
                                                                                    typeof(string)));

            testImporter.AddExtractConfiguration(new SimpleCSVExtractConfiguration(4,
                                                                                    "SampleDepth",
                                                                                    parserFactory.GetCellParser(typeof(double?)),
                                                                                    simpleValueAssigner,
                                                                                    typeof(double?)));

            testImporter.AddExtractConfiguration(new SimpleCSVExtractConfiguration(6,
                                                                                    "MatrixType",
                                                                                    parserFactory.GetCellParser(typeof(string)),
                                                                                    simpleValueAssigner,
                                                                                    typeof(string)));

            testImporter.AddExtractConfiguration(new SimpleCSVExtractConfiguration(7,
                                                                                    "SampleType",
                                                                                    parserFactory.GetCellParser(typeof(string)),
                                                                                    simpleValueAssigner,
                                                                                    typeof(string)));

            testImporter.AddExtractConfiguration(new SimpleCSVExtractConfiguration(8,
                                                                                    "ParentSample",
                                                                                    parserFactory.GetCellParser(typeof(string)),
                                                                                    simpleValueAssigner,
                                                                                    typeof(string)));

            testImporter.AddExtractConfiguration(new SimpleCSVExtractConfiguration(10,
                                                                                    "SDG",
                                                                                    parserFactory.GetCellParser(typeof(string)),
                                                                                    simpleValueAssigner,
                                                                                    typeof(string)));

            testImporter.AddExtractConfiguration(new SimpleCSVExtractConfiguration(11,
                                                                                    "LabName",
                                                                                    parserFactory.GetCellParser(typeof(string)),
                                                                                    simpleValueAssigner,
                                                                                    typeof(string)));

            testImporter.AddExtractConfiguration(new SimpleCSVExtractConfiguration(12,
                                                                                    "LabSampleID",
                                                                                    parserFactory.GetCellParser(typeof(string)),
                                                                                    simpleValueAssigner,
                                                                                    typeof(string)));

            testImporter.AddExtractConfiguration(new SimpleCSVExtractConfiguration(13,
                                                                                    "Comments",
                                                                                    parserFactory.GetCellParser(typeof(string)),
                                                                                    simpleValueAssigner,
                                                                                    typeof(string)));

            testImporter.AddExtractConfiguration(new SimpleCSVExtractConfiguration(14,
                                                                                    "LabReportNumber",
                                                                                    parserFactory.GetCellParser(typeof(string)),
                                                                                    simpleValueAssigner,
                                                                                    typeof(string)));
            return testImporter;
        }

        private static void AddXMLExtractConfigurationsToImporter(IDataImporter dataImporter)
        {
            var parserFactory = new DefaultXMLParserFactory();
            var labNameFieldExtractConfiguration = new SimpleXMLExtractConfiguration("", "Lab_Name", parserFactory.GetElementParser(typeof(string)), new SimpleValueAssigner(), typeof(string), "LabName");

            var dateReportedFieldExtractConfiguration = new SimpleXMLExtractConfiguration("", "Date_Reported", parserFactory.GetElementParser(typeof(DateTime)), new SimpleValueAssigner(), typeof(DateTime), "DateReported");

            var projectIDFieldExtractConfiguration = new SimpleXMLExtractConfiguration("", "Project_ID", parserFactory.GetElementParser(typeof(int)), new SimpleValueAssigner(), typeof(int), "ProjectId");

            var sdgIDFieldExtractConfiguration = new SimpleXMLExtractConfiguration("", "SDG_ID", parserFactory.GetElementParser(typeof(int)), new SimpleValueAssigner(), typeof(int), "SDGID");

            var labSignatoryFieldExtractConfiguration = new SimpleXMLExtractConfiguration("", "Lab_Signatory", parserFactory.GetElementParser(typeof(string)), new SimpleValueAssigner(), typeof(string), "LabSignatory");

            dataImporter.AddExtractConfiguration(labNameFieldExtractConfiguration);
            dataImporter.AddExtractConfiguration(dateReportedFieldExtractConfiguration);
            dataImporter.AddExtractConfiguration(projectIDFieldExtractConfiguration);
            dataImporter.AddExtractConfiguration(sdgIDFieldExtractConfiguration);
            dataImporter.AddExtractConfiguration(labSignatoryFieldExtractConfiguration);
        }
    }
}