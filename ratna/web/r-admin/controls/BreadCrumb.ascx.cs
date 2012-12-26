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
namespace Jardalu.Ratna.Web.Admin.controls
{
    #region using

    using System;
    using System.Collections.Generic;
    using System.Web.UI;
    using System.Web.UI.HtmlControls;

    #endregion

    public partial class BreadCrumb : System.Web.UI.UserControl
    {
        #region private fields

        private List<Pair> pairs = new List<Pair>();

        #endregion

        #region public methods

        public void Add(string name, string url)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("name");
            }

            pairs.Add(new Pair(name, url));
        }

        #endregion

        #region protected methods

        protected void Page_Load(object sender, EventArgs e)
        {
            if (pairs != null && pairs.Count > 0)
            {
                for(int i=0; i<pairs.Count; i++)
                {
                    HtmlGenericControl control = new HtmlGenericControl("li");

                    Pair pair = pairs[i];


                    HtmlAnchor anchor = new HtmlAnchor();
                    anchor.HRef = pair.Second as string;
                    anchor.InnerText = pair.First as string;

                    // assign last anchor id
                    if (i == pairs.Count - 1)
                    {
                        anchor.ID = "__breadcrumb_lastanchor";
                    }

                    control.Controls.Add(anchor);
                   

                    this.breadcrumb.Controls.Add(control);
                }

                HtmlGenericControl emptySpan = new HtmlGenericControl("span");
                emptySpan.InnerHtml = "&nbsp;";
                this.breadcrumb.Controls.Add(emptySpan);

            }
        }

        #endregion

    }
}
