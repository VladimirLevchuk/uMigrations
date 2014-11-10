
using System.Web.Mvc;
using Umbraco.Web.Models;

namespace uMigrations.MvcTest.Controllers
{
    public class Dt1Controller : Base.PageControllerBase
    {
        public override ActionResult Index(RenderModel model)
        {
            return CurrentTemplate(CurrentPage);
        }
    }
}