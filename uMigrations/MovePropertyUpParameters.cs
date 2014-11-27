using System.Collections.Generic;
using System.Linq;

namespace uMigrations
{
    public class MovePropertyUpParameters
    {
        public MovePropertyUpParameters(string destinationTypeAlias, 
            IEnumerable<string> sourceTypes, 
            string propertyAlias,
            string tabName = null, 
            bool mandatory = false, 
            object defaultValue = null, 
            INoValueAnalyzer noValueAnalyzer = null)
        {
            NoValueAnalyzer = noValueAnalyzer ?? new DefaultNoValueAnalyzer();
            DefaultValue = defaultValue;
            TabName = tabName;
            Mandatory = mandatory;
            PropertyAlias = propertyAlias;
            SourceTypes = sourceTypes.ToList().AsReadOnly();
            DestinationTypeAlias = destinationTypeAlias;
        }

        public string DestinationTypeAlias { get; private set; }
        public IReadOnlyCollection<string> SourceTypes { get; private set; }
        public string PropertyAlias { get; private set; }
        public string TabName { get; private set; }
        public bool Mandatory { get; private set; }
        public object DefaultValue { get; private set; }
        public INoValueAnalyzer NoValueAnalyzer { get; private set; }

        public override string ToString()
        {
            var sourceTypesString = string.Join(", ", SourceTypes.Select(x => "'" + x + "'"));
            
            return string.Format("Destination Type: '{0}', Source Types: {{{1}}}, Property: '{2}', Tab: '{3}',  Mandatory: {4}, Default Value: {5}, No Value Analyzer: {6}", 
                DestinationTypeAlias, 
                sourceTypesString, 
                PropertyAlias,
                TabName ?? "<null>", 
                Mandatory, 
                DefaultValue != null ? "with default value" : "without default value",
                NoValueAnalyzer);
        }
    }
}