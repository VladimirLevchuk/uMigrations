using Vega.USiteBuilder;

namespace uMigrations.MvcTest.Models.DataTypes
{
    [DataType(Name = CustomEditors.MultiContentPicker, PropertyEditorAlias = "Umbraco.MultiNodeTreePicker")]
    public class MultiContentPicker : DataTypeBase
    {
        public override DataTypePrevalue[] Prevalues
        {
            get
            {
                return new[]
                {
                    new DataTypePrevalue("startNode", "{    \"type\": \"content\"  }"), 
                    new DataTypePrevalue("filter", null), 
                    new DataTypePrevalue("minNumber", null), 
                    new DataTypePrevalue("maxNumber", null), 
                };
            }
        }
    }
}