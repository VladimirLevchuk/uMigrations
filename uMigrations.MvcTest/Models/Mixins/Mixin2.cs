using Vega.USiteBuilder;

namespace uMigrations.MvcTest.Models.Mixins
{
    public class Mixin2 : MixinBase
    {
        [DocumentTypeProperty(UmbracoPropertyType.Textstring, Name = "Mixin 2 Property 1", Tab = TabNames.Settings, Mandatory = true)]
        public virtual string Mixin2Prop1 { get; set; }

        [DocumentTypeProperty(UmbracoPropertyType.Textstring, Name = "Mixin 2 Property 2", Tab = TabNames.Content, Mandatory = false)]
        public virtual string Mixin2Prop2 { get; set; }

        [DocumentTypeProperty(UmbracoPropertyType.Textstring, Name = "Mixin 2 Property 3", Tab = TabNames.Content, DefaultValue = "default value")]
        public virtual string Mixin2Property3 { get; set; }

        [DocumentTypeProperty(UmbracoPropertyType.Textstring, Name = "Mixin 2 Property 4", Description = "description")]
        public virtual string Mixin2Property4 { get; set; }
    }
}