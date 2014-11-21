using System.Collections.Generic;
using System.Linq;

namespace uMigrations
{
    public class MovePropertyDownParameters
    {
        public MovePropertyDownParameters(string sourceTypeAlias, IEnumerable<string> destinationTypes)
        {
            DestinationTypes = destinationTypes.ToList().AsReadOnly();
            SourceTypeAlias = sourceTypeAlias;
        }

        public string SourceTypeAlias { get; private set; }
        public IReadOnlyCollection<string> DestinationTypes { get; private set; }
    }
}