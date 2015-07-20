using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.IO;

using AutoMapper;
using log4net;
using log4net.Config;

using Hatfield.EnviroData.MVC.AutoMapper;
using Hatfield.EnviroData.MVC.Infrastructure;

namespace Hatfield.EnviroData.MVC
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class WebApiApplication : System.Web.HttpApplication
    {
        private static readonly ILog log = LogManager.GetLogger("Application");

        protected void Application_Start()
        {
            RouteTable.Routes.MapHubs();
            XmlConfigurator.ConfigureAndWatch(new FileInfo(Path.Combine(Server.MapPath("/"), "log4net.config")));
            log.Info("Application Started");

            AreaRegistration.RegisterAllAreas();
            //Register the dependency
            DependencyConfigure.Initialize();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            //Create auto mapper
            AutoMapperConfiguration.Configure();
            Mapper.AssertConfigurationIsValid();

        }
    }
}