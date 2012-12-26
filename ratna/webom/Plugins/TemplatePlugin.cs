/*
    Copyright (c) 2012, Jardalu LLC. (http://jardalu.com)
        
    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
  
    For complete licensing, see license.txt or visit http://ratnazone.com/v0.2/license.txt

*/
namespace Jardalu.Ratna.Web.Templates
{
    #region using

    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    using Jardalu.Ratna.Core;
    using Jardalu.Ratna.Plugins;
    using Jardalu.Ratna.Store;
    using Jardalu.Ratna.Exceptions;
    using Jardalu.Ratna.Exceptions.ErrorCodes;
    using Jardalu.Ratna.Utilities;
    using Jardalu.Ratna.Web.Resource;

    #endregion

    public class TemplatePlugin : SystemPlugin
    {

        class SiteTemplateCache
        {
            public int SiteId;
            public bool IsCurrent;
            public SortedDictionary<string, Template> Templates = new SortedDictionary<string, Template>();
            public List<Template> CachedList = new List<Template>();
        }

        #region private fields

        private static TemplatePlugin instance = null;
        private static object syncRoot = new object();

        private static Dictionary<int, SiteTemplateCache> cache = new Dictionary<int, SiteTemplateCache>();
        private static Logger logger = Logger.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #endregion

        #region ctor

        public TemplatePlugin()
        {
            this.Name = "Template Plugin";
            this.Id = new Guid("d668791f-9377-40ea-9e38-b6ddf12c94fc");

            this.Register();
            this.Activate();
        }

        #endregion

        #region public properties

        public static TemplatePlugin Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                        {
                            instance = new TemplatePlugin();
                        }
                    }
                }

                return instance;
            }
        }

        public override DataContractSerializer Serializer
        {
            get
            {
                return null;
            }
        }

        public override string Type
        {
            get
            {
                return this.GetType().ToString();
            }
        }

        #endregion

        #region public methods

        public static string GetTemplateVirtualPath(string templatePath)
        {
            if (string.IsNullOrEmpty(templatePath))
            {
                throw new ArgumentNullException("templatePath");
            }

            string virtualPath = System.IO.Path.Combine(SitePaths.Template, templatePath);

            return virtualPath;
        }

        public IList<Template> GetTemplates()
        {
            SiteTemplateCache templateCache = GetSiteCache();

            if (!templateCache.IsCurrent)
            {
                lock (syncRoot)
                {
                    if (!templateCache.IsCurrent)
                    {
                        IList<Template> templates = PluginStore.Instance.Read<Template>(this, Template.KeyName);
                        templateCache.Templates.Clear();
                        templateCache.CachedList.Clear();

                        if (templates != null)
                        {
                            foreach (Template template in templates)
                            {
                                templateCache.CachedList.Add(template);
                                logger.Log(LogLevel.Debug, "Adding template [{0}] to cache list", template.Name);
                            }
                        }

                        templateCache.IsCurrent = true;
                    }
                }
            }

            return templateCache.CachedList;
        }

        /// <summary>
        /// Rules to match a template.
        ///     The last matching path and the template that is active.
        /// </summary>
        /// <param name="urlPath">Path on which the template is sought for</param>
        /// <returns>The template, null if no template found</returns>
        public Template GetActiveTemplate(string urlPath)
        {
            #region argument

            if (string.IsNullOrEmpty(urlPath))
            {
                throw new ArgumentNullException("urlPath");
            }

            #endregion

            Template template = null;

            SiteTemplateCache templateCache = GetSiteCache();

            if (!templateCache.Templates.ContainsKey(urlPath))
            {
                IList<Template> templates = GetTemplates();
                if (templates != null && templates.Count > 0)
                {
                    foreach (Template t in templates)
                    {
                        // check only for templates that are active
                        if (t.IsActivated &&
                            urlPath.StartsWith(t.UrlPath, StringComparison.OrdinalIgnoreCase))
                        {
                            // the last matching path.
                            template = t;

                            logger.Log(LogLevel.Debug, "Selecting template [{0}] for path [{1}]", template.Name, urlPath);
                        }
                    }
                }

                lock (syncRoot)
                {
                    if (!templateCache.Templates.ContainsKey(urlPath))
                    {
                        templateCache.Templates[urlPath] = template;
                    }
                }
            }
            else
            {
                template = templateCache.Templates[urlPath];
            }

            return template;
        }

        public Template GetTemplateUrlPath(string urlPath)
        {
            #region argument

            if (string.IsNullOrEmpty(urlPath))
            {
                throw new ArgumentNullException("urlPath");
            }

            #endregion

            Template template = null;

            IList<Template> templates = GetTemplates();
            if (templates != null && templates.Count > 0)
            {
                foreach (Template t in templates)
                {
                    if (t.UrlPath.Equals(urlPath, StringComparison.OrdinalIgnoreCase))
                    {
                        template = t;
                        break;
                    }
                }
            }

            return template;
        }

        public Template GetTemplate(string templatePath)
        {
            #region argument

            if (string.IsNullOrEmpty(templatePath))
            {
                throw new ArgumentNullException("templatePath");
            }

            #endregion

            Template template = null;

            IList<Template> templates = GetTemplates();
            if (templates != null && templates.Count > 0)
            {
                foreach (Template t in templates)
                {
                    if (t.TemplatePath.Equals(templatePath, StringComparison.OrdinalIgnoreCase))
                    {
                        template = t;
                        break;
                    }
                }
            }

            return template;
        }

        public void Add(Template template)
        {
            #region argument checking

            if (template == null)
            {
                throw new ArgumentNullException("template");
            }

            if (!template.IsValid())
            {
                throw new InvalidOperationException("template is invalid");
            }

            #endregion

            try
            {
                // add the template
                PluginStore.Instance.Save(this, template);

                Invalidate();
            }
            catch (MessageException me)
            {
                if (me.ErrorNumber == PluginErrorCodes.IdAlreadyInUse)
                {
                    // throw with a new message
                    throw new MessageException(
                            TemplateErrorCodes.PathAlreadyInUse,
                            ResourceManager.GetLiteral("Admin.Templates.Upload.PathAlreadyInUse")
                        );

                }

                throw;
            }
        }

        public void Delete(Guid uid)
        {

            PluginStore.Instance.Delete(this, uid);
            Invalidate();
        }

        public void Activate(Template template)
        {
            if (template == null)
            {
                throw new ArgumentNullException("template");
            }

            if (!template.IsActivated)
            {
                template.IsActivated = true;

                Save(template);

                Invalidate();
            }
        }

        public void Deactivate(Template template)
        {
            if (template == null)
            {
                throw new ArgumentNullException("template");
            }

            if (template.IsActivated)
            {
                template.IsActivated = false;
                Save(template);

                Invalidate();
            }
        }

        #endregion

        #region private methods

        private SiteTemplateCache GetSiteCache()
        {
            SiteTemplateCache templateCache = null;
            int siteId = WebContext.Current.Site.Id;

            if (!cache.ContainsKey(siteId))
            {
                lock (syncRoot)
                {
                    if (!cache.ContainsKey(siteId))
                    {
                        templateCache = new SiteTemplateCache();
                        templateCache.SiteId = siteId;
                        cache[siteId] = templateCache;
                    }
                }
            }

            templateCache = cache[siteId];

            return templateCache;
        }

        private void Invalidate()
        {
            SiteTemplateCache templateCache = GetSiteCache();
            lock (syncRoot)
            {
                templateCache.Templates.Clear();
                templateCache.CachedList.Clear();
                templateCache.IsCurrent = false;
            }
        }

        #endregion

    }

}
