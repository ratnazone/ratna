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
namespace Jardalu.Ratna.Web.UI
{
    #region using

    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Web;
    using System.Web.UI;

    using Jardalu.Ratna.Web.UI.Snippets;

    #endregion

    public class FormlessPage : Page
    {

        public FormlessPage()
        {
            Page.EnableEventValidation = false;
        }

        public override void VerifyRenderingInServerForm(Control control)
        {
        }

        public static Control GetControl(string virtualPath)
        {
            if (string.IsNullOrEmpty(virtualPath))
            {
                throw new ArgumentNullException("virtualPath");
            }

            FormlessPage fpage = new FormlessPage();
            return fpage.LoadControl(virtualPath);
        }


    }

}
