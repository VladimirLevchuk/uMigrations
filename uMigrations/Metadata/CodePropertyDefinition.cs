using System;

namespace uMigrations.Metadata
{
    public class CodePropertyDefinition : PropertyDefinition
    {
        public Type CustomConverter { get; set; }
    }
}