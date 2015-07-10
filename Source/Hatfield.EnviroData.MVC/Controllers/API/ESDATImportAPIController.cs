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
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace Hatfield.EnviroData.MVC.Controllers
{
    public class ESDATImportAPIController : ApiController
    {
        private IDbContext _dbContext;

        public ESDATImportAPIController(IDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpPost]
        public IEnumerable<ResultMessageViewModel> ImportLocalFiles()
        {
            var request = HttpContext.Current.Request;

            var headerFile = request.Files[0];
            var sampleFile = request.Files[1];
            var chemistryFile = request.Files[2];

            var headerFileData = new DataFromFileSystem(headerFile.FileName, headerFile.InputStream);
            var headerFileXMLFileToImport = new XMLDataToImport(headerFileData);

            var sampleFileData = new DataFromFileSystem(sampleFile.FileName, sampleFile.InputStream);
            var sampleFileCSVFileToImport = new CSVDataToImport(sampleFileData);

            var chemistryFileData = new DataFromFileSystem(chemistryFile.FileName, chemistryFile.InputStream);
            var chemistryCSVFileToImport = new CSVDataToImport(chemistryFileData);

            var esdatDataToImport = new ESDATDataToImport(headerFileXMLFileToImport, sampleFileCSVFileToImport, chemistryCSVFileToImport);
            var importer = ESDATDataImportHelper.BuildESDATDataImporter();

            var results = PersistESDATData(esdatDataToImport, importer);

            return results;
        }

        [HttpPost]
        public IEnumerable<ResultMessageViewModel> ImportHttpFiles(HttpFileImportDataViewModel data)
        {
            var headerFileHttpFileSystem = new HttpFileSystem(data.HeaderFileURL);
            var headerFileXMLFileToImport = new XMLDataToImport(headerFileHttpFileSystem.FetchData());

            var sampleFileHttpFileSystem = new HttpFileSystem(data.SampleFileURL);
            var sampleFileCSVFileToImport = new CSVDataToImport(sampleFileHttpFileSystem.FetchData());

            var chemistryFileHttpFileSystem = new HttpFileSystem(data.ChemistryFileURL);
            var chemistryCSVFileToImport = new CSVDataToImport(chemistryFileHttpFileSystem.FetchData());

            var esdatDataToImport = new ESDATDataToImport(headerFileXMLFileToImport, sampleFileCSVFileToImport, chemistryCSVFileToImport);
            var importer = ESDATDataImportHelper.BuildESDATDataImporter();

            var results = PersistESDATData(esdatDataToImport, importer);

            return results;
        }

        [HttpPost]
        public IEnumerable<ResultMessageViewModel> ImportFtpFiles(FtpFileImportDataViewModel data)
        {
            var headerFileFtpFileSystem = new FTPFileSystem(data.HeaderFileURL, data.UserName, data.Password);
            var headerFileXMLFileToImport = new XMLDataToImport(headerFileFtpFileSystem.FetchData());

            var sampleFileFtpFileSystem = new FTPFileSystem(data.SampleFileURL, data.UserName, data.Password);
            var sampleFileCSVFileToImport = new CSVDataToImport(sampleFileFtpFileSystem.FetchData());

            var chemistryFileFtpFileSystem = new FTPFileSystem(data.ChemistryFileURL, data.UserName, data.Password);
            var chemistryCSVFileToImport = new CSVDataToImport(chemistryFileFtpFileSystem.FetchData());

            var esdatDataToImport = new ESDATDataToImport(headerFileXMLFileToImport, sampleFileCSVFileToImport, chemistryCSVFileToImport);
            var importer = ESDATDataImportHelper.BuildESDATDataImporter();

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
                                  {
                                      Level = parsingResult.Level.ToString(),
                                      Message = parsingResult.Message
                                  };

                return failResults;
            }
            else
            {
                
                foreach (var entity in extractedResults.ExtractedEntities)
                {
                    var parameters = new ESDATSampleCollectionParameters(_dbContext, entity);
                    var converter = new ESDATConverter();
                    var action = converter.Convert(parameters);
                    _dbContext.Add(action);
                }
                
                //_dbContext.SaveChanges();

                return new List<ResultMessageViewModel> {
                    new ResultMessageViewModel{
                        Level = "Info",
                        Message = "Import success"
                    }
                };
            }
        }
    }
}