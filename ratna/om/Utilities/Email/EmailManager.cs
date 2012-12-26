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
namespace Jardalu.Ratna.Utilities.Email
{

    #region using

    using System;
    using System.Net.Mail;

    #endregion

    public class EmailManager
    {
        #region Private Fields

        private SmtpClient smtpClient;
        private static object syncRoot = new object();
        private EmailConfiguration configuration;

        private static Logger logger;

        #endregion

        #region ctor

        static EmailManager()
        {
            logger = Logger.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        }

        public EmailManager(EmailConfiguration configuration)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException("configuration");
            }

            this.configuration = configuration;

            Init(configuration);
        }

        #endregion

        private SmtpClient SmtpClient
        {
            get
            {
                return smtpClient;
            }
        }

        public void Send(string address, string subject, string body)
        {

            logger.Log( LogLevel.Debug, "Send called for address [{0}] with subject [{1}]", address, subject);

            try
            {
                MailMessage message = new MailMessage(configuration.FromAddress, address);
                message.Subject = subject;
                message.Body = body;
                message.IsBodyHtml = true;
                Send(message);
            }
            catch (Exception exception)
            {
                logger.Log(LogLevel.Error, "Unable to send email to {0}, exception : {1}", address, exception);
            }
        }

        public void Send(MailMessage message)
        {
            Send(String.Empty, message);
        }

        public void Send(string emailId, MailMessage message)
        {
            Send(false, emailId, message);
        }

        public void SendAsync(string emailId, MailMessage message)
        {
            Send(true, emailId, message);
        }

        public void SendAsync(string emailId, string address, string subject, string body)
        {

            logger.Log(LogLevel.Debug, "Send called for address [{0}] with subject [{1}]", address, subject);

            try
            {
                MailMessage message = new MailMessage(configuration.FromAddress, address);
                message.Subject = subject;
                message.Body = body;
                message.IsBodyHtml = true;
                SendAsync(emailId, message);
            }
            catch (Exception exception)
            {
                logger.Log(LogLevel.Error, "Unable to send email to {0}, exception : {1}", address, exception);
            }
        }

        #region Private Methods

        private void Init(EmailConfiguration configuration)
        {
            if (smtpClient == null)
            {
                lock (syncRoot)
                {
                    if (smtpClient == null)
                    {
                        smtpClient = new SmtpClient();
                        smtpClient.Host = configuration.SmtpAddress;
                        smtpClient.Credentials = configuration.Credentials;

                        smtpClient.SendCompleted += new SendCompletedEventHandler(smtpClient_SendCompleted);
                    }
                }
            }
        }

        private void Send(bool async, string emailId, MailMessage message)
        {
            #region arguments check

            if (message == null)
            {
                throw new ArgumentNullException("message");
            }

            if (emailId == null)
            {
                throw new ArgumentNullException("emailId");
            }

            #endregion

            #region Validations

            // make sure that there is a receiver and email body
            if (message.To == null || message.To.Count < 1)
            {
                logger.Log(LogLevel.Error, "Email does not have a valid To address");
                return;
            }

            //subject validation
            if (string.IsNullOrEmpty(message.Subject))
            {
                logger.Log(LogLevel.Error, "Email does not have a valid subject");
                return;
            }

            //body validation
            if (string.IsNullOrEmpty(message.Body))
            {
                logger.Log(LogLevel.Error, "Email does not have a valid body");
                return;
            }

            #endregion

            message.From = new MailAddress(configuration.FromAddress);

            logger.Log(LogLevel.Debug, "Sending email [EmailId : {0}] to [{1}] with subject [{2}]", emailId, message.To, message.Subject);
            string token = GenerateUserToken(emailId);

            if (async)
            {
                SmtpClient.SendAsync(message, token);

                logger.Log(LogLevel.Debug, "Called SendAsync method");
            }
            else
            {
                try
                {
                    SmtpClient.Send(message);
                    logger.Log(LogLevel.Debug, "Called Send method for [token : {0}]", token);
                }
                catch (Exception exception)
                {
                    logger.Log(LogLevel.Error, "Unable to deliver message for [token : {0}] Exception : {1}", token, exception);
                }
            }
        }

        private void smtpClient_SendCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            string token = e.UserState as string;

            logger.Log(LogLevel.Debug, "smtpClient_SendCompleted called for [token : {0}]", token);

            if (e.Cancelled)
            {
                logger.Log(LogLevel.Error, "Email sending cancelled [token : {0}]", token);
            }
            else
            {
                if (e.Error != null)
                {
                    logger.Log(LogLevel.Error, "Unable to send email [token : {0}], Exception {0}", token, e.Error);
                }
                else
                {
                    logger.Log(LogLevel.Info, "Email sent [token : {0}]", token);
                }
            }
        }

        private static string GenerateUserToken(string emailId)
        {

            string id = emailId;

            if (string.IsNullOrEmpty(emailId))
            {
                id = Guid.NewGuid().ToString();
            }

            string token = string.Format("EmailId : {0}", id);
            return token;
        }

        #endregion

    }

}
