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
namespace Jardalu.Ratna.Core.Comments
{
    #region using

    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    using Jardalu.Ratna.Plugins;
    using Jardalu.Ratna.Store;
    using Jardalu.Ratna.Exceptions;
    using Jardalu.Ratna.Exceptions.ErrorCodes;
    using Jardalu.Ratna.Resource;

    #endregion

    public class CommentsPlugin : SystemPlugin
    {
        #region private fields

        private static CommentsPlugin instance = null;
        private static object syncRoot = new object();

        #endregion

        #region ctor

        public CommentsPlugin()
        {
            this.Name = "Comments";
            this.Id = new Guid("d78fb89b-bb66-45aa-9d21-001f3a0068bb");

            this.Register();
            this.Activate();
        }

        #endregion

        #region public properties

        public static CommentsPlugin Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                        {
                            instance = new CommentsPlugin();
                        }
                    }
                }

                return instance;
            }
        }

        public override DataContractSerializer Serializer
        {
            get
            {
                return null;
            }
        }

        public override string Type
        {
            get
            {
                return this.GetType().ToString();
            }
        }

        #endregion

        #region public methods

        public Comment Read(Guid uid)
        {
            return PluginStore.Instance.Read<Comment>(this, uid);
        }

        public Comment Read(string key, string id)
        {
            #region argument checking

            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException("key");
            }

            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException("id");
            }

            #endregion

            Comment comment = PluginStore.Instance.Read<Comment>(this, key, id);
            return comment;
        }

        public IList<Comment> Read(string key, bool asc)
        {
            #region argument checking

            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException("key");
            }

            #endregion

            IList<Comment> comments = PluginStore.Instance.Read<Comment>(this, key);

            // get rid of comments that are pending.
            List<Comment> filteredComments = new List<Comment>();
            foreach (Comment c in comments)
            {
                if (c.Approved)
                {
                    filteredComments.Add(c);
                }
            }

            if (asc)
            {
                filteredComments.Reverse();
            }

            return filteredComments;
        }

        public IDictionary<string, int> GetCommentsCount(IList<string> keys)
        {
            // returns the count of the comments for the given set of keys.
            if (keys == null)
            {
                throw new ArgumentNullException("keys");
            }

            return PluginStore.Instance.GetCount<Comment>(this, keys);

        }

        public void Add(Comment comment)
        {
            #region argument checking

            if (comment == null)
            {
                throw new ArgumentNullException("comment");
            }

            if (!comment.IsValid())
            {
                throw new InvalidOperationException("comment is invalid");
            }

            #endregion

            try
            {
                if (comment.Time == DateTime.MinValue)
                {
                    comment.Time = DateTime.Now;
                }

                // add the template
                PluginStore.Instance.Save(this, comment);
            }
            catch (MessageException me)
            {
                if (me.ErrorNumber == PluginErrorCodes.IdAlreadyInUse)
                {
                    // throw with a new message
                    throw new MessageException(me.ErrorNumber, ResourceManager.GetMessage(CommentMessages.IdAlreadyInUse));
                }

                throw;
            }
        }

        public void Delete(Guid uid)
        {
            PluginStore.Instance.Delete(this, uid);
        }

        public IList<Comment> GetComments(string query, int start, int count, out int total)
        {
            return this.GetComments(query, false, start, count, out total);
        }

        public IList<Comment> GetComments(string query, bool pending, int start, int count, out int total)
        {
            if ((start < 0) || (count < 0))
            {
                throw new ArgumentException("start or count");
            }

            PluginDataQueryParameter parameter = new PluginDataQueryParameter();
            parameter.PropertyName = "approved";
            parameter.PropertyValue = "true";
            if (pending)
            {
                parameter.PropertyValue = "false";
            }

            List<PluginDataQueryParameter> parameters = new List<PluginDataQueryParameter>(2);
            parameters.Add(parameter);

            if (!string.IsNullOrEmpty(query))
            {
                parameter = new PluginDataQueryParameter("body", query);
                parameters.Add(parameter);
            }

            return PluginStore.Instance.Read<Comment>(this, parameters.ToArray(), start, count, false, out total);
        }

        #endregion
    }

}
