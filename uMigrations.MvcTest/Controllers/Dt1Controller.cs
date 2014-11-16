
using System.Collections.Generic;
using System.Web.Mvc;
using umbraco;
using Umbraco.Core.Models;
using Umbraco.Core.Services;
using Umbraco.Web.Models;

namespace uMigrations.MvcTest.Controllers
{
    public class Dt1Controller : Base.PageControllerBase
    {
        public override ActionResult Index(RenderModel model)
        {
            foreach (var contentType in Services.ContentTypeService.GetAllContentTypes())
            {
                ProcessContentType(contentType);
            }

            return CurrentTemplate(CurrentPage);
        }

        private void ProcessContentType(IContentType contentType)
        {
            var compositionGroups = contentType.CompositionPropertyGroups;
            var propertyGroups = contentType.PropertyGroups;
            var propertyTypes = contentType.PropertyTypes;
            var path = contentType.Path;
            var parentId = contentType.ParentId;
            if (parentId > 0)
            {
                var p = Services.ContentTypeService.GetContentType(parentId);
            }

            var contents = Services.ContentService.GetContentOfContentType(contentType.Id);

            ProcessContents(contents);
        }

        private void ProcessContents(IEnumerable<IContent> contents)
        {
            foreach (var content in contents)
            {
                var data = content.AdditionalData;
            }
        }
    }
}