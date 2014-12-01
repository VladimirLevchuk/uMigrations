using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vega.USiteBuilder;

namespace uMigrations.Tests.features.MovePropertyUp.Models
{
    [DocumentType(IconUrl = "doc4.gif",
        Thumbnail = "doc.png",
        Description = "Decription of Level1",
        AllowAtRoot = true,
        AllowedTemplates = new string[] { },
        DefaultTemplate = null,
        AllowedChildNodeTypes = new Type[] { })]
    public class Level1 : DocumentTypeBase
    {
        [DocumentTypeProperty(UmbracoPropertyType.Textstring,
            Name = "Level 1 Property 1", Mandatory = false)]
        public string Level1Prop1 { get; set; }

        [DocumentTypeProperty(UmbracoPropertyType.Textstring,
            Name = "Level 1 Property 2", Mandatory = false)]
        public virtual string Level1Prop2 { get; set; }
    }

    [DocumentType(IconUrl = "doc4.gif",
        Thumbnail = "doc.png",
        Description = "Decription of Level1",
        AllowedTemplates = new string[] { },
        DefaultTemplate = null,
        AllowedChildNodeTypeOf = new[] { typeof(SiteBuilder_PartialSync.Level1), typeof(SiteBuilder_PartialSync.Level2) })]
    public class Level2 : SiteBuilder_PartialSync.Level1
    {
        [DocumentTypeProperty(UmbracoPropertyType.Textstring,
            Name = "Level 2 Property 1", Mandatory = false)]
        public string Level2Prop1 { get; set; }

        [DocumentTypeProperty(UmbracoPropertyType.Textstring,
            Name = "Level 2 Property 2", Mandatory = false)]
        public virtual string Level2Prop2 { get; set; }
    }
}
