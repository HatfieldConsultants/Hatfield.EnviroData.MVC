using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

using Moq;
using NUnit.Framework;

using Hatfield.EnviroData.MVC.Controllers;

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
