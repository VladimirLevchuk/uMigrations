﻿using System;
using uMigrations.MvcTest.Models.Mixins;
using Vega.USiteBuilder;

namespace uMigrations.MvcTest.Models.DocumentTypes
{
    [DocumentType(IconUrl = "doc4.gif",
        Thumbnail = "folder.png",
        Description = "Decription of SecondLevelDT1",
        Mixins = new [] { typeof(Mixin1), typeof(Mixin2) },
        AllowedChildNodeTypeOf = new Type[] { typeof(FirstLevelDT) })]
    public class SecondLevelDT1 : FirstLevelDT
    {
        [DocumentTypeProperty(UmbracoPropertyType.Textstring, Name = "Property 1", Alias = "property1", Tab = TabNames.Settings, Mandatory = true)]
        public string Prop1 { get; set; }

        [DocumentTypeProperty(UmbracoPropertyType.Textstring, Name = "Property 2", Tab = TabNames.Content, Mandatory = false)]
        public string Prop2 { get; set; }

        [DocumentTypeProperty(UmbracoPropertyType.Textstring, Name = "Property 3", Tab = TabNames.Content, DefaultValue = "default value")]
        public string Property3 { get; set; }

        [DocumentTypeProperty(UmbracoPropertyType.Textstring, Name = "Property 4", Description = "description")]
        public string Property4 { get; set; }

        [MixinProperty]
        public virtual Mixin1 Mixin1 { get; set; }

        [MixinProperty]
        public Mixin2 Mixin2 { get; set; }
    }
}