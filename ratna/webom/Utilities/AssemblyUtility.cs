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
namespace Jardalu.Ratna.Web.Utilities
{
    #region using

    using System;
    using System.Reflection;

    using Jardalu.Ratna.Utilities;
    using Jardalu.Ratna.Exceptions;
    using Jardalu.Ratna.Web.Resource;

    #endregion

    public static class AssemblyUtility
    {

        #region private fields

        private static Logger logger;

        #endregion

        #region ctor

        static AssemblyUtility()
        {
            logger = Logger.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        }

        #endregion

        
        public static Assembly Load(string assemblyFile)
        {
            if (string.IsNullOrEmpty(assemblyFile))
            {
                throw new ArgumentNullException("assemblyFile");
            }

            Assembly assembly = null;

            try
            {
                assembly = Assembly.LoadFrom(assemblyFile);
            }
            catch (Exception exception)
            {
                // log the message
                logger.Log(LogLevel.Warn, "Unable to load assembly from : {0}. Exception - {1}", assemblyFile, exception);

                throw new MessageException(
                        string.Format(ResourceManager.GetLiteral("Admin.Apps.InvalidAppAssembly"), assemblyFile)
                    );
            }

            return assembly;
        }

    }

}
