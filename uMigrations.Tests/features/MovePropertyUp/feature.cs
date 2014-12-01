using NSpec;
using Umbraco.Core;
using Umbraco.Core.Persistence;
using Umbraco.Core.Services;

namespace uMigrations.Tests.features.MovePropertyUp
{
    public abstract class feature : nspec
    {
        public virtual Database Db { get { return ApplicationContext.Current.DatabaseContext.Database; } }

        public virtual ServiceContext Services { get { return ApplicationContext.Current.Services; } }
    }
}