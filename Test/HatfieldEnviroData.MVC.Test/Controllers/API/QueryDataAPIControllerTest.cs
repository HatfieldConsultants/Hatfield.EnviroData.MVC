using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using AutoMapper;
using Moq;
using NUnit.Framework;

using Hatfield.EnviroData.Core;
using Hatfield.EnviroData.MVC.Models;
using Hatfield.EnviroData.DataAcquisition.ESDAT;
using Hatfield.EnviroData.MVC.Controllers.API;
using Hatfield.EnviroData.MVC.AutoMapper;

namespace HatfieldEnviroData.MVC.Test.Controllers.API
{
    [TestFixture]
    public class QueryDataAPIControllerTest
    {
        [TestFixtureSetUp]
        public void Init()
        {
            AutoMapperConfiguration.Configure();
        }

        [Test]
        public void GetTest()
        {
            var mockRepository = new Mock<IActionRepository>();

            mockRepository.Setup(x => x.GetAllSampleCollectionActions())
                          .Returns(() => new List<Hatfield.EnviroData.Core.Action> { 
                            new Hatfield.EnviroData.Core.Action{
                                ActionID = 2,
                                BeginDateTime = new DateTime(2015, 2, 2)
                            },
                            new Hatfield.EnviroData.Core.Action{
                                ActionID = 3,
                                BeginDateTime = new DateTime(2015, 2, 3)
                            }
                          });
            var controller = new QueryDataAPIController(mockRepository.Object);

            var viewModels = controller.Get();
                        
            Assert.NotNull(viewModels);
            Assert.AreEqual(2, viewModels.Count());

            var firstModel = viewModels.ElementAt(0);
            Assert.AreEqual(2, firstModel.Id);
            Assert.AreEqual("Feb-02-2015", firstModel.Name);

            var secondModel = viewModels.ElementAt(1);
            Assert.AreEqual(3, secondModel.Id);
            Assert.AreEqual("Feb-03-2015", secondModel.Name);
        }
    }
}
