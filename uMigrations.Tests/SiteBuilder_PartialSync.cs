using System;
using FluentAssertions;
using NUnit.Framework;
using Umbraco.Core;
using Vega.USiteBuilder;
using Vega.USiteBuilder.DocumentTypeBuilder.Contracts;

namespace uMigrations.Tests
{
    public class SiteBuilder_PartialSync
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
            AllowedChildNodeTypeOf = new[] { typeof(Level1), typeof(Level2) })]
        public class Level2 : Level1
        {
            [DocumentTypeProperty(UmbracoPropertyType.Textstring,
                Name = "Level 2 Property 1", Mandatory = false)]
            public string Level2Prop1 { get; set; }

            [DocumentTypeProperty(UmbracoPropertyType.Textstring,
                Name = "Level 2 Property 2", Mandatory = false)]
            public virtual string Level2Prop2 { get; set; }
        }

        [Test]
        public void Test()
        {
            var docTypeManager = new DocumentTypeManager();
            docTypeManager.SynchronizeDocumentTypes(new [] { typeof(Level1), typeof(Level2) });

            var service = ApplicationContext.Current.Services.ContentTypeService;
            
            var l1Type = service.GetContentType("Level1");
            var l2Type = service.GetContentType("Level2");

            l1Type.Should().NotBeNull();
            l2Type.Should().NotBeNull();
        }
    }
}