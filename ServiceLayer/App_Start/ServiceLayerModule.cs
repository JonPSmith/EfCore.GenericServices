// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using Autofac;

namespace ServiceLayer
{
    public class ServiceLayerModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            //Autowire the classes with interfaces
            builder.RegisterAssemblyTypes(GetType().Assembly).AsImplementedInterfaces();

            //-----------------------------
            //Now register the other layers
            //builder.RegisterModule(new BizDbAccessModule());
            //builder.RegisterModule(new BizLogicModule());
        }
    }

}