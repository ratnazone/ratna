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
namespace Jardalu.Ratna.Web.Applications
{

    #region using

    using System;

    using Jardalu.Ratna.Core.Apps;
    using Jardalu.Ratna.Core.Comments;
    using Jardalu.Ratna.Core.Forms;
    using Jardalu.Ratna.Utilities;

    #endregion

    internal static class AppExecutor
    {

        #region private fields

        private static Logger logger = Logger.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #endregion

        #region public methods

        public static void Execute(AbstractApp app, AppEvent appEvent, object eventObject)
        {

            if (app == null)
            {
                throw new ArgumentNullException("app");
            }

            if (appEvent == AppEvent.None)
            {
                logger.Log(LogLevel.Info, "Ignoring execution on {0} as event is none", app);
                return;
            }

            Comment comment;
            FormEntry formEntry;

            logger.Log(LogLevel.Info, "Executing [{0}] for event - [{1}]", app, appEvent);

            switch (appEvent)
            {
                case AppEvent.CommentSaved:
                    comment = eventObject as Comment;
                    app.CommentSaved(comment);
                    break;

                case AppEvent.CommentSaving:
                    comment = eventObject as Comment;
                    app.CommentSaving(comment);
                    break;

                case AppEvent.CommentSpammed:
                    comment = eventObject as Comment;
                    app.CommentSpamed(comment);
                    break;

                case AppEvent.FormEntrySaving:
                    formEntry = eventObject as FormEntry;
                    app.FormEntrySaving(formEntry);
                    break;

                case AppEvent.FormEntrySaved:
                    formEntry = eventObject as FormEntry;
                    app.FormEntrySaved(formEntry);
                    break;

                case AppEvent.ImageUploaded:
                    app.ImageUploaded(eventObject as string);
                    break;
            }

            logger.Log(LogLevel.Info, "Execution [{0}] for event - [{1}] done", app, appEvent);
        }

        #endregion
    }

}
