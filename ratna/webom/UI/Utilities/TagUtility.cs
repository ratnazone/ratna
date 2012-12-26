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
namespace Jardalu.Ratna.Web.UI.Utilities
{
    #region using

    using System;
    using System.IO;
    using System.Text;
    using System.Web;
    using System.Web.Routing;
    using System.Web.UI;
    using System.Web.UI.HtmlControls;

    using Jardalu.Ratna.Core;
    using Jardalu.Ratna.Core.Articles;

    #endregion

    public class TagUtility
    {
        #region fields

        public const int MaxTagsDisplay = 5;

        #endregion

        #region public methods

        public static string GetTagsHtml(Article article)
        {
            string tagsHtml = null;

            if (article != null && article.Tags != null && article.Tags.Count > 0)
            {
                int i = 0;
                StringBuilder builder = new StringBuilder();
                using (HtmlTextWriter writer = new HtmlTextWriter(new StringWriter(builder)))
                {
                    foreach (Tag tag in article.Tags)
                    {
                        if (i == MaxTagsDisplay)
                        {
                            break;
                        }

                        if (i != 0)
                        {
                            builder.Append(", ");
                        }

                        //display the tag link
                        HtmlAnchor anchor = new HtmlAnchor();
                        anchor.HRef = TagUtility.GetTagLink(tag);
                        anchor.InnerText = tag.Name;
                        anchor.RenderControl(writer);

                        i++;
                    }
                }

                tagsHtml = builder.ToString();
            }

            return tagsHtml;

        }
        
        /// <summary>
        /// Returns the URL associated with the tag.
        /// </summary>
        /// <param name="tag"></param>
        /// <returns>url for tag</returns>
        public static string GetTagLink(Tag tag)
        {
            #region arguments

            if (tag == null)
            {
                throw new ArgumentNullException("tag");
            }

            #endregion

            string tagLink = "";

            DynamicPage dPage = HttpContext.Current.Handler as DynamicPage;
            if (dPage != null)
            {

                RouteValueDictionary rvd = new RouteValueDictionary();
                rvd.Add(Constants.PageRouteIdentifier, 1);

                if (!string.IsNullOrEmpty(tag.Name))
                {
                    rvd.Add(Constants.SearchRouteIdentifier, tag.Name);
                }

                tagLink = dPage.GetRouteUrl(dPage.RouteName, rvd);
            }

            return tagLink;
        }

        #endregion

    }

}
