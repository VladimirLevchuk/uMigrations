using Umbraco.Core.Models;
using Vega.USiteBuilder;

namespace uMigrations.MvcTest.Models.DataTypes
{
    [DataType(Name = CustomEditors.MultiMediaPicker
        , PropertyEditorAlias = "Umbraco.MultipleMediaPicker"
        , DataTypeDatabaseType = DataTypeDatabaseType.Nvarchar)]
    public class MultiMediaPicker : DataTypeBase
    {
        public override DataTypePrevalue[] Prevalues
        {
            get
            {
                return new[]
                {
                    new DataTypePrevalue("multiPicker", "1")
                };
            }
        }
    }
}