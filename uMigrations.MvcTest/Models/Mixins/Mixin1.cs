using Vega.USiteBuilder;

namespace uMigrations.MvcTest.Models.Mixins
{
    public class Mixin1 : MixinBase
    {
        [DocumentTypeProperty(UmbracoPropertyType.Textstring, Name = "Mixin 1 Property 1", Tab = TabNames.Settings, Mandatory = true)]
        public string Mixin1Prop1 { get; set; }

        [DocumentTypeProperty(UmbracoPropertyType.Textstring, Name = "Mixin 1 Property 2", Tab = TabNames.Content, Mandatory = false)]
        public string Mixin1Prop2 { get; set; }

        [DocumentTypeProperty(UmbracoPropertyType.Textstring, Name = "Mixin 1 Property 3", Tab = TabNames.Content, DefaultValue = "default value")]
        public string Mixin1Property3 { get; set; }

        [DocumentTypeProperty(UmbracoPropertyType.Textstring, Name = "Mixin 1 Property 4", Description = "description")]
        public string Mixin1Property4 { get; set; }
    }
}