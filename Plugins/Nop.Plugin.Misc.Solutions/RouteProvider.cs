using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Routing;
using System.Diagnostics;
using Nop.Web.Framework.Mvc.Routes;


namespace Nop.Plugin.Misc.Solutions
{
    public partial class RouteProvider : IRouteProvider
    {
        public void RegisterRoutes(RouteCollection routes)
        {
            routes.MapRoute("Plugin.Misc.Solutions.Router",
                "Solutions/{*url}",
                new { controller = "Solutions", action = "Router" },
                new[] { "Nop.Plugin.Misc.Solutions.Controllers" });
        }
        public int Priority
        {
            get
            {
                return 0;
            }
        }
    }
}
