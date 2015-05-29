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

using Hatfield.EnviroData.Core;
using Hatfield.EnviroData.Core.Repositories;

namespace Hatfield.EnviroData.MVC.Infrastructure
{
    internal class DependencyConfigure
    {
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

            builder.RegisterAssemblyTypes(
                typeof(WebApiApplication).Assembly
                ).PropertiesAutowired();

            //deal with your dependencies here
            builder.RegisterType<ODM2Entities>().As<IDbContext>().InstancePerDependency();
            builder.RegisterGeneric(typeof(Repository<>)).As(typeof(IRepository<>));
            builder.RegisterType<VariableRepository>().As<IVariableRepository>();
            builder.RegisterType<ActionRepository>().As<IActionRepository>();


            return builder.Build();
        }
    }
}