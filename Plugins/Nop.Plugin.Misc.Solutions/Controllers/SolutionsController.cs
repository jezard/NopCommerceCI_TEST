using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Topics;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Services.Stores;
using Nop.Services.Topics;
using Nop.Web.Framework.Controllers;
using Nop.Web.Infrastructure.Cache;
using Nop.Web.Models.Topics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;


namespace Nop.Plugin.Misc.Solutions.Controllers
{
    public class SolutionsController : BasePluginController
    {

        private readonly ITopicService _topicService;
        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;
        private readonly ILocalizationService _localizationService;
        private readonly ICacheManager _cacheManager;
        private readonly IStoreMappingService _storeMappingService;
        private readonly IAclService _aclService;
        private readonly ITopicTemplateService _topicTemplateService;

        private Dictionary<string, string> _solutionsBC = new Dictionary<string, string>();


        public SolutionsController(ITopicService topicService,
            ILocalizationService localizationService,
            IWorkContext workContext, 
            IStoreContext storeContext,
            ICacheManager cacheManager,
            IStoreMappingService storeMappingService,
            IAclService aclService,
            ITopicTemplateService topicTemplateService)
        {
            this._topicService = topicService;
            this._workContext = workContext;
            this._storeContext = storeContext;
            this._localizationService = localizationService;
            this._cacheManager = cacheManager;
            this._storeMappingService = storeMappingService;
            this._aclService = aclService;
            this._topicTemplateService = topicTemplateService;
        }

        [NonAction]
        protected virtual TopicModel PrepareTopicModel(Topic topic)
        {
            if (topic == null)
                throw new ArgumentNullException("topic");

            var model = new TopicModel
            {
                Id = topic.Id,
                SystemName = topic.SystemName,
                IncludeInSitemap = topic.IncludeInSitemap,
                IsPasswordProtected = topic.IsPasswordProtected,
                Title = topic.IsPasswordProtected ? "" : topic.GetLocalized(x => x.Title),
                Body = topic.IsPasswordProtected ? "" : topic.GetLocalized(x => x.Body),
                MetaKeywords = topic.GetLocalized(x => x.MetaKeywords),
                MetaDescription = topic.GetLocalized(x => x.MetaDescription),
                MetaTitle = topic.GetLocalized(x => x.MetaTitle),
                SeName = topic.GetSeName(),
                TopicTemplateId = topic.TopicTemplateId
            };
            return model;
        }

        public ActionResult Configure()
        {
            return View("~/Plugins/Misc.Solutions/Views/Solutions/Configure.cshtml");
        }
        

        public ActionResult Router(string url)
        {
            //start our url with the main topic name
            List<string> t = new List<string>();
            t.Add("Solutions");

            //if not root solutions index
            if (url != null)
            {
                string[] s = url.Split('/');

                foreach (var item in s)
                {//don't add an empty item caused by trailing slash
                    if (!String.IsNullOrEmpty(item))
                    {
                        t.Add(item);
                    }
                }
            }

            string[] segments = t.ToArray();
            Page page = new Page(segments);

            //if a user tries to navigate to a parent which isn't there for example by editing url in browser
            if (page.SystemName == null)
            {

                //try going up a level to find a topic there
                int firstNullIndex = 0;
                foreach (string segment in segments)
                {
                    if (segment == null)
                    {
                        break;
                    }
                    firstNullIndex++;
                }
                segments[firstNullIndex - 1] = null;
                page = new Page(segments);
               
            }
            if (page.SystemName != null)
            {
                ViewBag.PageName = page.PageName;
                ViewBag.Parent = page.Parent;
                ViewBag.GrandParent = page.GrandParent;
                ViewBag.Siblings = page.Siblings;
                ViewBag.ParentChildren = page.ParentChildren.ToArray();
                ViewBag.SystemName = page.SystemName;
                ViewBag.ParentSiblings = page.ParentSiblings().ToArray();
                ViewBag.GrandParentSiblings = page.GrandParentSiblings().ToArray();
                ViewBag.Children = page.Children().ToArray();

                return PartialView(String.Format("~/Plugins/Misc.Solutions/Views/Templates/{0}.cshtml", page.TemplateType));
            }

            //default
            return PartialView("~/Plugins/Misc.Solutions/Views/Solutions/Shared/NoContent.cshtml");
        }

        /// <summary>
        /// Return a template part for a given reference
        /// </summary>
        /// <param name="systemName">Topic System name (created in admin)</param>
        /// <returns>View, a partial html page</returns>
        public ActionResult TopicPartial(string systemName)
        {
            var cacheKey = string.Format(ModelCacheEventConsumer.TOPIC_MODEL_BY_SYSTEMNAME_KEY,
                systemName,
                _workContext.WorkingLanguage.Id,
                _storeContext.CurrentStore.Id,
                string.Join(",", _workContext.CurrentCustomer.GetCustomerRoleIds()));
            var cacheModel = _cacheManager.Get(cacheKey, () =>
            {
                var topic = _topicService.GetTopicBySystemName(systemName, _storeContext.CurrentStore.Id);
                if (topic == null)
                    return null;

                return PrepareTopicModel(topic);
            }
            );

            if (cacheModel == null)
                return PartialView("~/Plugins/Misc.Solutions/Views/Solutions/Shared/NoContent.cshtml");

            //template
            var templateCacheKey = string.Format(ModelCacheEventConsumer.TOPIC_TEMPLATE_MODEL_KEY, cacheModel.TopicTemplateId);
            var templateViewPath = _cacheManager.Get(templateCacheKey, () =>
            {
                var template = _topicTemplateService.GetTopicTemplateById(cacheModel.TopicTemplateId);
                if (template == null)
                    template = _topicTemplateService.GetAllTopicTemplates().FirstOrDefault();
                if (template == null)
                    throw new Exception("No default template could be loaded");
                return template.ViewPath;
            });

            return PartialView(templateViewPath, cacheModel);
        }
    }
}