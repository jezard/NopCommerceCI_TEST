using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Routing;
using Nop.Core.Plugins;
using Nop.Services.Common;
using System.Web.Mvc;


namespace Nop.Plugin.Misc.Solutions
{
    public class Solutions: BasePlugin, IMiscPlugin
    {
        /// <summary>
        /// Gets a route for provider configuration
        /// </summary>
        /// <param name="actionName">Action name</param>
        /// <param name="controllerName">Controller name</param>
        /// <param name="routeValues">Route values</param>
        public void GetConfigurationRoute(out string actionName, out string controllerName, out RouteValueDictionary routeValues)
        {
            actionName = "Configure";
            controllerName = "Solutions";
            routeValues = new RouteValueDictionary { { "Namespaces", "Nop.Plugin.Misc.Solutions.Controllers" }, { "area", null } };
        }
    }
}
