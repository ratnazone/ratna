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
namespace Jardalu.Ratna.Utilities
{
    #region using

    using System;
    using System.Diagnostics;

    using Jardalu.Ratna.Core;

    using log4net;
    using log4net.Repository;
    using log4net.Appender;

    #endregion

    public enum LogLevel
    {
        Debug = 0,
        Info = 1,
        Warn = 2,
        Error = 3,
        Fatal = 4
    }

    public class Logger
    {
        #region private fields

        private ILog logger;
        private static LogLevel enabledLevel = LogLevel.Debug;
        private static bool isEnabled;

        private const string MessageFormat = @"[StickyId : {0}] [SiteId : {1}] {2}";

        #endregion

        #region ctor

        private Logger(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            this.logger = LogManager.GetLogger(type);
        }

        #endregion

        #region public properties

        public static LogLevel EnabledLevel
        {
            get
            {
                return enabledLevel;
            }
            set
            {
                enabledLevel = value;
            }
        }

        public static bool IsEnabled
        {
            get
            {
                return isEnabled;
            }
            set
            {
                if (value != isEnabled)
                {
                    if (value)
                    {
                        //enable log
                        log4net.Config.XmlConfigurator.Configure();

                    }
                    else
                    {
                        //close
                        Close();
                    }

                    isEnabled = value;
                }
            }
        }

        public static bool TraceOn
        {
            get;
            set;
        }

        #endregion

        #region public methods

        public static Logger GetLogger(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            Logger log = new Logger(type);
            return log;
        }

        public void Log(string message)
        {
            Log(LogLevel.Debug, message);
        }

        public void Log(LogLevel level, string format, params object[] args)
        {
            if (format == null)
            {
                throw new ArgumentNullException("format");
            }

            Log(level, string.Format(format, args));
        }

        public void Log(LogLevel level, string message)
        {
            if (level >= EnabledLevel)
            {
                Guid stickyId = WebContext.Current.StickyId;
                int siteId = WebContext.Current.Site.Id;

                string formattedMessage = string.Format(MessageFormat, stickyId, siteId, message);

                switch (level)
                {
                    case LogLevel.Debug:
                        logger.Debug(formattedMessage);
                        break;
                    case LogLevel.Info:
                        logger.Info(formattedMessage);
                        break;
                    case LogLevel.Warn:
                        logger.Warn(formattedMessage);
                        break;
                    case LogLevel.Error:
                        logger.Error(formattedMessage);
                        break;
                    case LogLevel.Fatal:
                        logger.Fatal(formattedMessage);
                        break;
                }

                if (TraceOn)
                {
                    Trace.WriteLine(formattedMessage);
                }
            }
        }

        public static void Close()
        {
            if (IsEnabled)
            {
                ILoggerRepository rep = LogManager.GetRepository();
                foreach (IAppender appender in rep.GetAppenders())
                {
                    var buffered = appender as BufferingAppenderSkeleton;
                    if (buffered != null)
                    {
                        buffered.Flush();
                    }

                    appender.Close();
                }

            }
        }

        #endregion

    }

}
