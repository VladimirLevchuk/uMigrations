using System;
using System.Collections.Generic;

namespace uMigrations
{
    public abstract class MovePropertyBase : IMigrationAction
    {
        public abstract List<Exception> Validate();
        public abstract void Run();
    }
}