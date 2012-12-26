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

    using Jardalu.Ratna.Utilities.Email;
    using Jardalu.Ratna.Web.Plugins;
    using Jardalu.Ratna.Utilities;

    #endregion

    public class Notifier
    {
        #region private fields

        private static Logger logger;

        #endregion

        #region ctor

        static Notifier()
        {
            logger = Logger.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        }

        #endregion

        public static void Notify(string subject, string body)
        {

            if (string.IsNullOrEmpty(subject))
            {
                throw new ArgumentNullException("subject");
            }

            if (string.IsNullOrEmpty(body))
            {
                throw new ArgumentNullException("body");
            }

            NotificationConfiguration config = NotificationConfiguration.Read();
            NotifyNewComment(config, subject, body);
        }

        #region private methods

        public static void NotifyNewComment(NotificationConfiguration config, string subject, string body)
        {
            
            if (config.NotifyOnComment &&
                !string.IsNullOrEmpty(config.NotifyToEmail))
            {
                //check for notificaiton email.
                EmailConfiguration configuration = new EmailConfiguration();
                try
                {
                    configuration.Configure(config.SmtpAddress, config.SmtpUserName, config.SmtpPassword, config.FromAddress);
                    EmailManager manager = new EmailManager(configuration);
                    manager.SendAsync(string.Empty, config.NotifyToEmail, subject, body);
                }
                catch (ArgumentException)
                {
                    //not configured correctly
                    logger.Log(LogLevel.Debug, "Unable to notify on new comment. Please check SMTP settings.");
                }

            }
            else
            {
                logger.Log(LogLevel.Debug, "Unable to notify on new comment. NotifyOnComment is off or receiver email address is not set.");
            }

        }

        #endregion

    }
}
