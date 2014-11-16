using System.Collections.Generic;

namespace uMigrations.Metadata
{
    /// <summary>
    /// Document 
    /// </summary>
    public class DocumentTypeDefinition
    {
        public DocumentTypeDefinition Base { get; set; }
        /// <summary>
        /// Gets or sets display name
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// Gets or sets alias (identifier)
        /// </summary>
        public string Alias { get; set; }

        /// <summary>
        /// Gets or sets document type property definitions
        /// </summary>
        public virtual List<PropertyDefinition> PropertyDefinitions { get; set; }
    }
}