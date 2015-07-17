using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

using Hatfield.EnviroData.Core;
using Hatfield.EnviroData.DataAcquisition;
using Hatfield.EnviroData.DataAcquisition.CSV;
using Hatfield.EnviroData.DataAcquisition.ESDAT;
using Hatfield.EnviroData.DataAcquisition.ESDAT.Converters;
using Hatfield.EnviroData.DataAcquisition.XML;
using Hatfield.EnviroData.FileSystems;
using Hatfield.EnviroData.FileSystems.FTPFileSystem;
using Hatfield.EnviroData.FileSystems.HttpFileSystem;
using Hatfield.EnviroData.MVC.Helpers;
using Hatfield.EnviroData.MVC.Models;
using Hatfield.EnviroData.WQDataProfile;

namespace Hatfield.EnviroData.MVC.Controllers
{
    public class ESDATImportAPIController : ApiController
    {
        private static string HeaderFileInputElementName = "headerFileInput";
        private static string SampleFileInputElementName = "sampleFileInput";
        private static string ChemistryFileInputElementName = "chemistryFileInput";

        private IDbContext _dbContext;
        private IWQDefaultValueProvider _wqDefaultValueProvider;

        WayToHandleNewData wayToHandleNewData = WayToHandleNewData.CreateInstanceForNewData;

        public ESDATImportAPIController(IDbContext dbContext, IWQDefaultValueProvider wqDefaultValueProvider)
        {
            _dbContext = dbContext;
            _wqDefaultValueProvider = wqDefaultValueProvider;
        }

        [HttpPost]
        public IEnumerable<ResultMessageViewModel> ImportLocalFiles()
        {
            var results = new List<ResultMessageViewModel>();

            var request = HttpContext.Current.Request;

            XMLDataToImport headerFileXMLFileToImport = null;

            if (request.Files.AllKeys.Contains(HeaderFileInputElementName))
            {
                var headerFile = request.Files[HeaderFileInputElementName];
                var headerFileData = new DataFromFileSystem(headerFile.FileName, headerFile.InputStream);
                headerFileXMLFileToImport = new XMLDataToImport(headerFileData);
            }
            else
            {
                results.Add(new ResultMessageViewModel(ResultMessageViewModel.RESULT_LEVEL_WARN, "Header file is not uploaded, use default value."));
            }
            
            var sampleFile = request.Files[SampleFileInputElementName];
            var chemistryFile = request.Files[ChemistryFileInputElementName];

            var sampleFileData = new DataFromFileSystem(sampleFile.FileName, sampleFile.InputStream);
            var sampleFileCSVFileToImport = new CSVDataToImport(sampleFileData);

            var chemistryFileData = new DataFromFileSystem(chemistryFile.FileName, chemistryFile.InputStream);
            var chemistryCSVFileToImport = new CSVDataToImport(chemistryFileData);

            var esdatDataToImport = new ESDATDataToImport(headerFileXMLFileToImport, sampleFileCSVFileToImport, chemistryCSVFileToImport);
            var importer = ESDATDataImportHelper.BuildESDATDataImporter(_wqDefaultValueProvider);

            results.AddRange(PersistESDATData(esdatDataToImport, importer));

            return results;
        }

        [HttpPost]
        public IEnumerable<ResultMessageViewModel> ImportHttpFiles(HttpFileImportDataViewModel data)
        {
            XMLDataToImport headerFileXMLFileToImport = null;

            if (!string.IsNullOrEmpty(data.HeaderFileURL))
            {
                var headerFileHttpFileSystem = new HttpFileSystem(data.HeaderFileURL);
                headerFileXMLFileToImport = new XMLDataToImport(headerFileHttpFileSystem.FetchData());
            }
            

            var sampleFileHttpFileSystem = new HttpFileSystem(data.SampleFileURL);
            var sampleFileCSVFileToImport = new CSVDataToImport(sampleFileHttpFileSystem.FetchData());

            var chemistryFileHttpFileSystem = new HttpFileSystem(data.ChemistryFileURL);
            var chemistryCSVFileToImport = new CSVDataToImport(chemistryFileHttpFileSystem.FetchData());

            var esdatDataToImport = new ESDATDataToImport(headerFileXMLFileToImport, sampleFileCSVFileToImport, chemistryCSVFileToImport);
            var importer = ESDATDataImportHelper.BuildESDATDataImporter(_wqDefaultValueProvider);

            var results = PersistESDATData(esdatDataToImport, importer);

            return results;
        }

        [HttpPost]
        public IEnumerable<ResultMessageViewModel> ImportFtpFiles(FtpFileImportDataViewModel data)
        {
            XMLDataToImport headerFileXMLFileToImport = null;

            if (!string.IsNullOrEmpty(data.HeaderFileURL))
            {
                var headerFileHttpFileSystem = new FTPFileSystem(data.HeaderFileURL);
                headerFileXMLFileToImport = new XMLDataToImport(headerFileHttpFileSystem.FetchData());
            }

            var sampleFileFtpFileSystem = new FTPFileSystem(data.SampleFileURL, data.UserName, data.Password);
            var sampleFileCSVFileToImport = new CSVDataToImport(sampleFileFtpFileSystem.FetchData());

            var chemistryFileFtpFileSystem = new FTPFileSystem(data.ChemistryFileURL, data.UserName, data.Password);
            var chemistryCSVFileToImport = new CSVDataToImport(chemistryFileFtpFileSystem.FetchData());

            var esdatDataToImport = new ESDATDataToImport(headerFileXMLFileToImport, sampleFileCSVFileToImport, chemistryCSVFileToImport);
            var importer = ESDATDataImportHelper.BuildESDATDataImporter(_wqDefaultValueProvider);

            var results = PersistESDATData(esdatDataToImport, importer);

            return results;
        }

        private IEnumerable<ResultMessageViewModel> PersistESDATData(ESDATDataToImport esdatDataToImport, IDataImporter importer)
        {
            var extractedResults = importer.Extract<ESDATModel>(esdatDataToImport);

            if (!extractedResults.IsExtractedSuccess)
            {
                var failResults = from parsingResult in ImportResultHelper.FilterWarningAndErrorResult(extractedResults.AllParsingResults)
                                  select new ResultMessageViewModel
                                  (
                                      parsingResult.Level.ToString(),
                                      parsingResult.Message
                                  );

                return failResults;
            }
            else
            {
                var esdatModel = extractedResults.ExtractedEntities.First();

                var duplicateChecker = new ESDATDuplicateChecker(_dbContext);
                var sampleCollectionFactory = new ESDATSampleCollectionMapperFactory(duplicateChecker, _wqDefaultValueProvider, wayToHandleNewData);


                var chemistryFactory = new ESDATChemistryMapperFactory(duplicateChecker, _wqDefaultValueProvider, wayToHandleNewData);

                var mapper = new SampleCollectionActionMapper(duplicateChecker, sampleCollectionFactory, _wqDefaultValueProvider, chemistryFactory, wayToHandleNewData);

                var converter = new ESDATConverter(mapper);

                var action = converter.Convert(esdatModel);

                _dbContext.Add<Hatfield.EnviroData.Core.Action>(action);
                _dbContext.SaveChanges();

                return new List<ResultMessageViewModel> {
                    new ResultMessageViewModel
                    (
                        ResultMessageViewModel.RESULT_LEVEL_INFO,
                        "Import success"
                    )
                };
            }
        }
    }
}