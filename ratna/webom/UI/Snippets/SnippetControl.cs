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
namespace Jardalu.Ratna.Web.UI.Snippets
{

    #region using

    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.IO;
    using System.Reflection;
    using System.Web.UI;
    using System.Globalization;

    #endregion

    public abstract class SnippetControl : System.Web.UI.UserControl
    {

        #region public methods

        public virtual void PopulateControl()
        {
        }

        public virtual void SetProperties(NameValueCollection collection)
        {
            if (collection != null)
            {
                string[] keys = collection.AllKeys;
                foreach (string key in keys)
                {
                    try
                    {
                        // set the property for the control.
                        PropertyInfo pInfo = this.GetType().GetProperty(key);
                        if (pInfo != null && 
                            pInfo.GetSetMethod() != null)
                        {

                            object value = Utility.FromString(collection[key], pInfo.PropertyType);

                            // set the property
                            this.GetType().InvokeMember(
                                                pInfo.Name, 
                                                BindingFlags.SetProperty, 
                                                null, 
                                                this, 
                                                new Object[] { value }
                                          );
                        }
                    }
                    catch
                    {
                        // ignore if no property found
                        // or ambigous property
                        // or the property value cannot be set with string
                    }
                }
            }

        }

        public string GetHtml()
        {
            // populate the control
            this.PopulateControl();

            StringWriter buffer = new StringWriter();
            HtmlTextWriter writer = new HtmlTextWriter(buffer);

            this.RenderControl(writer);
            writer.Flush();

            return buffer.ToString().Trim();
        }

        public string GetJsonHtml()
        {
            string html = GetHtml();
            html = Utility.SanitizeJsonHtml(html);
            return html;
        }

        #endregion

    }

}
