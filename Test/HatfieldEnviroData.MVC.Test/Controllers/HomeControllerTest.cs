using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

using Hatfield.EnviroData.MVC.Controllers;
using Moq;
using NUnit.Framework;

namespace HatfieldEnviroData.MVC.Test.Controllers
{
    [TestFixture]
    public class HomeControllerTest
    {
        [Test]
        public void IndexTest()
        {
            var homeController = new HomeController();
            var actionResult = homeController.Index();

            Assert.NotNull(actionResult);
        }
    }
}
