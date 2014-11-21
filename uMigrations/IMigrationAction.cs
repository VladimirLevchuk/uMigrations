using System;
using System.Collections.Generic;

namespace uMigrations
{
    public interface IMigrationAction
    {
        string ToString();
        List<Exception> Validate();
        void Run();
    }
}