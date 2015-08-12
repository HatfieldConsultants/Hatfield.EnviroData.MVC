using Hatfield.EnviroData.Core;
using Hatfield.EnviroData.MVC.AutoMapper;
using Hatfield.EnviroData.MVC.Controllers.API;
using Hatfield.EnviroData.MVC.Models;
using Hatfield.EnviroData.WQDataProfile;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HatfieldEnviroData.MVC.Test.Controllers.API
{
    [TestFixture]
    public class StationQueryAPIControllerTest
    {
        private StationQueryAPIController controller;

        [TestFixtureSetUp]
        public void Init()
        {
            AutoMapperConfiguration.Configure();
            var mockSiteRepository = new Mock<ISiteRepository>();
            var items = new List<Hatfield.EnviroData.Core.Site> { 
                            new Hatfield.EnviroData.Core.Site{
                                SiteTypeCV = "Stream",
                                Latitude = 49.2827,
                                Longitude = 123.1207,
                                SpatialReferenceID = 1
                            },
                            new Hatfield.EnviroData.Core.Site{
                                SiteTypeCV = "Stream",
                                Latitude = 49.2827,
                                Longitude = 123.1207,
                                SpatialReferenceID = 1
                            }};
            mockSiteRepository.Setup(x => x.GetAll())
                          .Returns(items.AsQueryable());

            var mockActionRepository = new Mock<IWQDataRepository>();
            var actions = new List<Hatfield.EnviroData.Core.Action> { 
             new Hatfield.EnviroData.Core.Action{
                                ActionID = 2,
                                BeginDateTime = new DateTime(2015, 2, 2)
                            },
                            new Hatfield.EnviroData.Core.Action{
                                ActionID = 3,
                                BeginDateTime = new DateTime(2015, 2, 3)
                            }               
            };
            mockSiteRepository.Setup(x => x.GetAll())
                          .Returns(items.AsQueryable());

            var mockVariableRepository = new Mock<IWQVariableRepository>();
            var variables = new List<Hatfield.EnviroData.Core.Variable> { 
                            new Hatfield.EnviroData.Core.Variable{
                                VariableTypeCV = "Chemistry",
                                VariableCode = "108-88-3",
                                VariableNameCV = "1,1,1-Trichloroethane",
                                VariableDefinition = "",
                                NoDataValue = -9999,
                            },
                            new Hatfield.EnviroData.Core.Variable{
                                VariableTypeCV = "Chemistry",
                                VariableCode = "528-78-3",
                                VariableNameCV = "1,1,1-Trichloroethane",
                                VariableDefinition = "",
                                NoDataValue = -9999,
                            }};
            mockVariableRepository.Setup(x => x.GetAllChemistryVariables())
                          .Returns(variables.AsQueryable());

            var mockDefaultValueProvider = new Mock<IWQDefaultValueProvider>();

            this.controller = new StationQueryAPIController(mockDefaultValueProvider.Object, mockSiteRepository.Object, mockVariableRepository.Object, mockActionRepository.Object);
        }



        [Test]
        public void GetSitesTest()
        {
            var sites = controller.GetSites();

            Assert.NotNull(sites);
            Assert.AreEqual(2, sites.Count());

            var firstSite = sites.ElementAt(0);
            Assert.AreEqual("Stream", firstSite.SiteTypeCV);
            Assert.AreEqual(123.1207, firstSite.Longitude);
        }

        [Test]
        public void GetAnalytesTest()
        {
            var analytes = controller.GetAllAnalytes();
            Assert.NotNull(analytes);
             Assert.AreEqual(2, analytes.Count());
            var firstAnalyte = analytes.ElementAt(0);
            Assert.AreEqual(firstAnalyte.VariableTypeCV, "Chemistry");

        }

        //[Test]
        //public void FetchStationDataTest()
        //{
        //    var viewModel = new FetchSiteAnalyteQueryViewModel{
        //StartDate = new DateTime(), EndDate = new DateTime(),SelectedSiteID = 1, SelectedVariables = new int[4]{1,2,3,4} };
        //    var queryResults = controller.FetchStationData(viewModel);
        //}
    }
}
