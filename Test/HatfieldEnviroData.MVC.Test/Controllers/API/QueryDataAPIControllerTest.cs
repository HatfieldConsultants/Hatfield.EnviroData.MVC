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
using Hatfield.EnviroData.WQDataProfile;

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

        [Test]
        public void GetSampleCollectionActionInESDATTest()
        {
            var mockRepository = new Mock<IActionRepository>();

            mockRepository.Setup(x => x.GetActionById(It.IsAny<int>()))
                          .Returns(() =>
                            new Hatfield.EnviroData.Core.Action
                            {
                                ActionID = 2,
                                BeginDateTime = new DateTime(2015, 2, 2), 
                                FeatureActions = new List<FeatureAction>{
                                    new FeatureAction{
                                                    Results = new List<Result>{
                                                        new Result{
                                                            ResultDateTime = new DateTime(2015, 2, 3),                                                            
                                                        }
                                                    }
                                                }
                                },
                                RelatedActions = new List<RelatedAction> { 
                                    new RelatedAction{
                                        Action1 = new Hatfield.EnviroData.Core.Action{
                                            BeginDateTime = new DateTime(2015, 2, 3),
                                            Method = new Method{
                                                MethodDescription = "test description",
                                                MethodName = "test method name"
                                            },
                                            FeatureActions = new List<FeatureAction>{
                                                new FeatureAction{
                                                    Results = new List<Result>{
                                                        new Result{
                                                            ResultDateTime = new DateTime(2015, 2, 3),
                                                            MeasurementResult = new MeasurementResult{
                                                                MeasurementResultValues = new List<MeasurementResultValue>{
                                                                    new MeasurementResultValue{
                                                                        ValueDateTime = new DateTime(2015, 2, 4),
                                                                        DataValue = 2.0
                                                                        
                                                                    },
                                                                    new MeasurementResultValue{
                                                                        ValueDateTime = new DateTime(2015, 2, 4),
                                                                        DataValue = 3.0
                                                                    }
                                                                },
                                                                Unit = new Unit{
                                                                    UnitsName = "test unit name"
                                                                }
                                                            },
                                                            Variable = new Variable{
                                                                VariableDefinition = "test variable full name"
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                          );

            var controller = new QueryDataAPIController(mockRepository.Object);

            var viewModel = controller.GetSampleCollectionActionInESDAT(1);

            Assert.NotNull(viewModel);

            Assert.AreEqual(new DateTime(2015, 2, 2), viewModel.DateReported);

            var samplingData = viewModel.SampleFileData;
            Assert.AreEqual(1, samplingData.Count());
            var firstSamplingData = samplingData.ElementAt(0);
            Assert.AreEqual(new DateTime(2015, 2, 3), firstSamplingData.SampledDateTime);

            var chemistryData = viewModel.ChemistryData;
            Assert.AreEqual(2, chemistryData.Count());

            var firstChemistryData = chemistryData.ElementAt(0);
            Assert.AreEqual(DateTime.MinValue, firstChemistryData.AnalysedDate);
            Assert.AreEqual(new DateTime(2015, 2, 3), firstChemistryData.ExtractionDate);
            Assert.AreEqual(2.0, firstChemistryData.Result);
            Assert.AreEqual("test unit name", firstChemistryData.ResultUnit);
            Assert.AreEqual("test variable full name", firstChemistryData.OriginalChemName);

            var secondChemistryData = chemistryData.ElementAt(1);
            Assert.AreEqual(DateTime.MinValue, secondChemistryData.AnalysedDate);
            Assert.AreEqual(new DateTime(2015, 2, 3), secondChemistryData.ExtractionDate);
            Assert.AreEqual(3.0, secondChemistryData.Result);
            Assert.AreEqual("test unit name", secondChemistryData.ResultUnit);
            Assert.AreEqual("test variable full name", secondChemistryData.OriginalChemName);

        }
    }
}
