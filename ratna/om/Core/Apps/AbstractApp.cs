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
namespace Jardalu.Ratna.Core.Apps
{
    #region using

    using System;

    using Jardalu.Ratna.Core.Comments;
    using Jardalu.Ratna.Core.Forms;

    #endregion

    /// <summary>
    /// Abstract Apps are apps that does not necessarily run in a page context.
    /// For example, when a comment is posted, this app can determine to automatically delete.
    /// </summary>
    public abstract class AbstractApp
    {

        /// <summary>
        /// The App that was invoked at the runtime.
        /// </summary>
        public App InvokedApp
        {
            get;
            set;
        }

        /// <summary>
        /// When a comment is posted, this method is called before the comment is saved.
        /// </summary>
        /// <param name="c">Comment that is being posted</param>
        public virtual void CommentSaving(Comment c)
        {
        }

        /// <summary>
        /// When a comment is posted, this method is called after the comment is saved.
        /// </summary>
        /// <param name="c">Comment that was posted</param>
        public virtual void CommentSaved(Comment c)
        {
        }

        /// <summary>
        /// When a comment is spamed, this method is called after the comment is spamed.
        /// </summary>
        /// <param name="c">Comment that was spamed</param>
        public virtual void CommentSpamed(Comment c)
        {
        }


        /// <summary>
        /// When a forms response is posted, this method is called before the entry
        /// is saved. Changes done by the app on the form entry will get persisted.
        /// </summary>
        /// <param name="fe">FormEntry</param>
        public virtual void FormEntrySaving(FormEntry fe)
        {
        }

        /// <summary>
        /// When a forms response is posted, this method is called after the entry
        /// is saved.  Changes done by the app on the form entry will NOT get persisted.
        /// </summary>
        /// <param name="fe">FormEntry</param>
        public virtual void FormEntrySaved(FormEntry fe)
        {
        }

        /// <summary>
        /// When an image is uploaded, this method is called. The app will know the location
        /// where the image was uploaded. The image can be modified (such as dimensions, adding water-marks
        /// etc ).
        /// </summary>
        /// <param name="file">full path of the uploaded image</param>
        public virtual void ImageUploaded(string file)
        {
        }

    }

}
