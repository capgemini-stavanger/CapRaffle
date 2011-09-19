using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ninject;
using CapRaffle.Domain.Abstract;
using CapRaffle.Domain.Implementation;

namespace CapRaffle.Infrastructure
{
    public class NinjectControllerFactory : DefaultControllerFactory
    {
        private IKernel ninjectKernel;

        public NinjectControllerFactory()
        {
            ninjectKernel = new StandardKernel();
            AddBindings();
        }

        protected override IController GetControllerInstance(System.Web.Routing.RequestContext requestContext, Type controllerType)
        {
            return controllerType == null ? null : (IController)ninjectKernel.Get(controllerType);
        }

        private void AddBindings()
        {
            //Add bindings here
            //eks: ninjectKernel.Bind<Interface>().To<ImplementationOfInterface>();

            ninjectKernel.Bind<ICategoryRepository>().To<CategoryRepository>();
        }
    }
}