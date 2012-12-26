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
namespace Jardalu.Ratna.Web.Admin
{
    #region using

    using System;

    using Jardalu.Ratna.Store;

    #endregion

    public partial class activateuser : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string code = Request["code"];
            string alias = Request["alias"];

            //by default assume user is not enabled
            this.successdiv.Visible = false;

            if (!string.IsNullOrEmpty(code) && !string.IsNullOrEmpty(alias))
            {
                //check if the code is valid
                bool activated = UserStore.Instance.ActivateUser(alias, code);
                if (activated)
                {
                    // user has been activated
                    this.errordiv.Visible = false;
                    this.successdiv.Visible = true;
                }
            }
        }
    }
}
