using System;
using System.Linq;
using System.Web;
using Vega.USiteBuilder;

namespace uMigrations.MvcTest.Models.DocumentTypes
{
    [DocumentType(IconUrl = "doc4.gif",
              Thumbnail = "folder.png",
              Description = "Decription of ThirdLevelDT",
              AllowedTemplates = new string[] { "ThirdLevel" },
              AllowedChildNodeTypes = new Type[] { typeof(FourthLevelDT) },
              AllowedChildNodeTypeOf = new Type[] { typeof(SecondLevelDT1) })]
    public class ThirdLevelDT : SecondLevelDT1
    {
    }
}