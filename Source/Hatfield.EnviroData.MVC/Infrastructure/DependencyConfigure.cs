using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Http;

using Autofac;
using Autofac.Core;
using Autofac.Integration.Mvc;
using Autofac.Integration.WebApi;
using log4net;

using Hatfield.EnviroData.Core;
using Hatfield.EnviroData.Core.Repositories;
using Hatfield.EnviroData.WQDataProfile;
using Hatfield.WQDefaultValueProvider.JSON;
using Hatfield.EnviroData.WQDataProfile.Repositories;

namespace Hatfield.EnviroData.MVC.Infrastructure
{
    internal class DependencyConfigure
    {
        private static readonly ILog log = LogManager.GetLogger("Application");

        public static void Initialize()
        {
            var builder = new ContainerBuilder();
            var container = RegisterServices(builder);

            // Set the dependency resolver for Web API.
            var webApiResolver = new AutofacWebApiDependencyResolver(container);
            GlobalConfiguration.Configuration.DependencyResolver = webApiResolver;

            // Set the dependency resolver for MVC.
            var mvcResolver = new AutofacDependencyResolver(container);
            DependencyResolver.SetResolver(mvcResolver);
        }

        private static IContainer RegisterServices(ContainerBuilder builder)
        {
            var jsonConfigFilePath = System.Web.Hosting.HostingEnvironment.MapPath("~/App_Data/DefaultValues.json");

            log.Debug("Fetch default value provider data from " + jsonConfigFilePath);

            builder.RegisterAssemblyTypes(
                typeof(WebApiApplication).Assembly
                ).PropertiesAutowired();

            //deal with your dependencies here
            builder.RegisterType<ODM2Entities>().As<IDbContext>().InstancePerDependency();
            builder.RegisterGeneric(typeof(Repository<>)).As(typeof(IRepository<>));
            builder.RegisterType<VariableRepository>().As<IVariableRepository>();
            builder.RegisterType<ActionRepository>().As<IActionRepository>();
            builder.RegisterType<SiteRepository>().As<ISiteRepository>();
            builder.RegisterType<ResultRepository>().As<IResultRepository>();
            builder.RegisterType<MeasurementResultValueRepository>().As<IMeasurementResultValueRepository>();
            builder.RegisterType<UnitRepository>().As<IUnitRepository>();
            builder.RegisterType<WQDataRepository>().As<IWQDataRepository>();
            //builder.RegisterType<JSONWQDefaultValueProvider>()
            //       .As<IWQDefaultValueProvider>()
            //       .WithParameter("jsonFilePath", jsonConfigFilePath)
            //       .WithParameter("createNewConfigFileIfNotExist", true)
            //       .WithParameter("wayToLoadConfigFile", WayToLoadConfigFile.CreateNewConfigFileIfLoadFail)
            //       .InstancePerLifetimeScope();

            builder.RegisterType<StaticWQDefaultValueProvider>()
                   .As<IWQDefaultValueProvider>()                   
                   .InstancePerLifetimeScope();
            return builder.Build();
        }
    }
}