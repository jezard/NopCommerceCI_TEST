using System;
using System.Collections.Generic;
using System.Linq;

using System.Web.Mvc;


using System.Web;

using Nop.Core.Data;
using Nop.Core.Caching;
using Nop.Core.Domain.Topics;
using Nop.Core.Infrastructure;

namespace Nop.Plugin.Misc.Solutions
{
    class TopicActionFilter :  ActionFilterAttribute, IFilterProvider
    {
        private readonly ICacheManager _cacheManager;

        public TopicActionFilter(
            ICacheManager cacheManager
            )
        {
            this._cacheManager = cacheManager;
        }


        public IEnumerable<Filter> GetFilters(ControllerContext controllerContext, ActionDescriptor actionDescriptor)
        {
            if (controllerContext.Controller is Nop.Admin.Controllers.TopicController && actionDescriptor.ActionName.Equals("Edit", StringComparison.InvariantCultureIgnoreCase))
            {
                return new List<Filter>() { new Filter(this, FilterScope.Action, 0) };
            }

            return new List<Filter>();
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {

            return;
        }
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            //This works to change the directory sought by the image uploader dependant on conditions e.g. a 
            //topic being created or edited
            //Does require the Nop Hacked code to be restored (fixpath() in RoxyFilemandController.cs) and conf.json to have the 
            //"SESSION_PATH_KEY" set to "DynamicDirectory"

            
            HttpContext context = HttpContext.Current;
           
            HttpRequest req = context.Request;
            var requrl = req.Url.ToString();

            var topicId = Convert.ToInt32(requrl.Split('/').Last());//get id
            string key = string.Format("Nop.topics.id-{0}", topicId);

            var topicRepository = EngineContext.Current.Resolve<IRepository<Topic>>();
            var topic = _cacheManager.Get(key, () => topicRepository.GetById(topicId));

            var systemName = topic.SystemName;

            //Only switch for solutions (test system name)
            if (systemName.Contains("Solutions."))
            {
                context.Session["DynamicDirectory"] = "~/Plugins/Misc.Solutions/Content/Solutions/Images/";
            }
            else
            {
                //default
                context.Session.Clear();
            }

            return;
        }
    }
   
}
