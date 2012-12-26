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
namespace Jardalu.Ratna.Web.Service
{

    #region using

    using System;
    using System.Web;
    using System.Web.Services;
    using System.Web.Script.Services;

    using Jardalu.Ratna.Utilities;
    using Jardalu.Ratna.Core.Forms;
    using Jardalu.Ratna.Web.Resource;
    using Jardalu.Ratna.Web.Utilities;
    using Jardalu.Ratna.Exceptions;
    using Jardalu.Ratna.Exceptions.ErrorCodes;
    using Jardalu.Ratna.Web.Admin.service;

    #endregion

    [ScriptService]
    public class forms : ServiceBase
    {

        #region private fields

        private Logger logger = Logger.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #endregion

        [WebMethod]
        [ScriptMethod(UseHttpGet = true)]
        public string AddReponse(string form, string fields)
        {
            ServiceOutput output = new ServiceOutput();
            output.Success = false;

            formsmanage.AddFormEntry(form, Guid.Empty, fields, output);

            return output.GetJson();
        }

    }
}
