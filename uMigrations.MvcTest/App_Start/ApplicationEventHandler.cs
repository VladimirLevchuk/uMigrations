using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Umbraco.Core;
using umbraco.interfaces;

namespace uMigrations.MvcTest
{
    public class ApplicationEventHandler : IApplicationEventHandler
    {
        public virtual void OnApplicationInitialized(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            
        }

        public virtual void OnApplicationStarting(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            
        }

        public virtual void OnApplicationStarted(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            Migrations.Run();
        }
    }
}