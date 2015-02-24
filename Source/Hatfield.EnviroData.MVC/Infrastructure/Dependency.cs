using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Autofac;

namespace Hatfield.EnviroData.MVC.Infrastructure
{
    public class Dependency : IDependencyResolver
    {
        private readonly IContainer container;

        public Dependency(IContainer container)
        {

            this.container = container;
        }

        public object GetService(Type serviceType)
        {
            return
                container.IsRegistered(serviceType)
                    ? container.Resolve(serviceType)
                    : null;
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {

            Type enumerableServiceType =
                typeof(IEnumerable<>).MakeGenericType(serviceType);

            object instance =
                container.Resolve(enumerableServiceType);

            return ((IEnumerable)instance).Cast<object>();
        }
    }
}