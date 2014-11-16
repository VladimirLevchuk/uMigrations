namespace uMigrations.Metadata
{
    /// <summary>
    /// Property metadata
    /// </summary>
    public class PropertyDefinition
    {
        public bool Mandatory { get; set; }

        public string Description { get; set; }

        public string ValidationExpression { get; set; }

        public object DefaultValue { get; set; }

        /// <summary>
        /// Gets or sets display name
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// Gets or sets alias (identifier)
        /// </summary>
        public string Alias { get; set; }

        /// <summary>
        /// Gets or sets property tab
        /// </summary>
        public TabDefinition Tab { get; set; }

        public DataTypeDefinition PropertyType { get; set; }
    }
}