using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

using Hatfield.EnviroData.Core;
using Hatfield.EnviroData.MVC.Models;
using Hatfield.EnviroData.FileSystems;
using Hatfield.EnviroData.FileSystems.HttpFileSystem;
using Hatfield.EnviroData.DataAcquisition.XML;
using Hatfield.EnviroData.DataAcquisition.CSV;
using Hatfield.EnviroData.DataAcquisition.ESDAT;
using Hatfield.EnviroData.MVC.Helpers;
using Hatfield.EnviroData.DataAcquisition;
using Hatfield.EnviroData.DataAcquisition.ESDAT.Converters;

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
                var esdatConverter = new ESDATConverter(_dbContext);
                var action = esdatConverter.ConvertToODMAction(extractedResults.ExtractedEntities.First());

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