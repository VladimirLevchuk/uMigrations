using System.Collections.Generic;
using System.Linq;

namespace uMigrations
{
    public interface INoValueAnalyzerProvider
    {
        INoValueAnalyzer NoValueAnalyzer { get; }
    }

    public class MovePropertyDownParameters
    {
        public MovePropertyDownParameters(string sourceTypeAlias, 
            IEnumerable<string> destinationTypes, 
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
            DestinationTypes = destinationTypes.ToList().AsReadOnly();
            SourceTypeAlias = sourceTypeAlias;
        }

        public string SourceTypeAlias { get; private set; }
        public IReadOnlyCollection<string> DestinationTypes { get; private set; }
        public string PropertyAlias { get; private set; }
        public string TabName { get; private set; }
        public bool Mandatory { get; private set; }
        public object DefaultValue { get; private set; }
        public INoValueAnalyzer NoValueAnalyzer { get; private set; }

        public override string ToString()
        {
            var destinationTypesString = string.Join(", ", DestinationTypes.Select(x => "'" + x + "'"));

            return string.Format("Source Type: '{0}', Destination Types: {{{1}}}, Property: '{2}', Tab: '{3}',  Mandatory: {4}, Default Value: {5}",
                SourceTypeAlias,
                destinationTypesString,
                PropertyAlias,
                TabName ?? "<null>",
                Mandatory,
                DefaultValue != null ? "with default value" : "without default value");
        }
    }
}