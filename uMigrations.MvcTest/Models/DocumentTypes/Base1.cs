using System;
using Vega.USiteBuilder;

namespace uMigrations.MvcTest.Models.DocumentTypes
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
        AllowedTemplates = new string[] {},
        DefaultTemplate = null,
        AllowedChildNodeTypeOf = new[] { typeof (Level1), typeof(Level2) })]
    public class Level2 : Level1
    {
        [DocumentTypeProperty(UmbracoPropertyType.Textstring,
            Name = "Level 2 Property 1", Mandatory = false)]
        public string Level2Prop1 { get; set; }

        [DocumentTypeProperty(UmbracoPropertyType.Textstring,
            Name = "Level 2 Property 2", Mandatory = false)]
        public virtual string Level2Prop2 { get; set; }        
    }


    [DocumentType(IconUrl = "doc4.gif",
        Thumbnail = "doc.png",
        Description = "Decription of Level1",
        AllowAtRoot = true,
        AllowedTemplates = new string[] { },
        DefaultTemplate = null,
        AllowedChildNodeTypes = new Type[] { })]
    public class Level11 : DocumentTypeBase
    {
        [DocumentTypeProperty(UmbracoPropertyType.Textstring,
            Name = "Level 1.1 Property 1", Mandatory = false)]
        public string Level1Prop1 { get; set; }

        [DocumentTypeProperty(UmbracoPropertyType.Textstring,
            Name = "Level 1.1 Property 2", Mandatory = false)]
        public virtual string Level1Prop2 { get; set; }
    }

    [DocumentType(IconUrl = "doc4.gif",
        Thumbnail = "doc.png",
        Description = "Decription of Level11",
        AllowedTemplates = new string[] { },
        DefaultTemplate = null,
        AllowedChildNodeTypeOf = new[] { typeof(Level11) })]
    public class Level21 : Level11
    {
        [DocumentTypeProperty(UmbracoPropertyType.Textstring,
            Name = "Level 2.1 Property 1", Mandatory = false)]
        public string Level21Prop1 { get; set; }

        [DocumentTypeProperty(UmbracoPropertyType.Textstring,
            Name = "Level 2.1 Property 2", Mandatory = false)]
        public virtual string Level21Prop2 { get; set; }

        [DocumentTypeProperty(UmbracoPropertyType.Textstring,
            Name = "Move to Base", Mandatory = false)]
        public virtual string MoveToBase { get; set; }

        [DocumentTypeProperty(UmbracoPropertyType.Textstring,
            Name = "Move to Base 2 (mandatory)", Mandatory = true)]
        public string MoveToBase2 { get; set; }
    }

    [DocumentType(IconUrl = "doc4.gif",
        Thumbnail = "doc.png",
        Description = "Decription of Level11",
        AllowedTemplates = new string[] { },
        DefaultTemplate = null,
        AllowedChildNodeTypeOf = new[] { typeof(Level11) })]
    public class Level22 : Level11
    {
        [DocumentTypeProperty(UmbracoPropertyType.Textstring,
            Name = "Level 2.2 Property 1", Mandatory = false)]
        public string Level22Prop1 { get; set; }

        [DocumentTypeProperty(UmbracoPropertyType.Textstring,
            Name = "Level 2.2 Property 2", Mandatory = false)]
        public virtual string Level22Prop2 { get; set; }

        [DocumentTypeProperty(UmbracoPropertyType.Textstring,
            Name = "Move to Base", Mandatory = false)]
        public virtual string MoveToBase { get; set; }

        [DocumentTypeProperty(UmbracoPropertyType.Textstring,
            Name = "Move to Base 2", Mandatory = false)]
        public string MoveToBase2 { get; set; }
    }
}