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
namespace Jardalu.Ratna.Core.Tags
{
    #region using

    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using System.Text;

    using Jardalu.Ratna.Store;

    #endregion

    [DataContract]
    public abstract class TagResource : Resource
    {

        #region private fields

        private List<Tag> tags = new List<Tag>();
        private Guid tagKey = Guid.Empty;

        private const char SerializedTagsDelimiter = ',';

        private bool isTagDirty;

        #endregion

        #region public properties

        public bool IsTagDirty
        {
            get 
            {
                return isTagDirty;
            }
            set
            {
                isTagDirty = value;
            }
        }

        public IList<Tag> Tags
        {
            get
            {
                return tags;
            }
        }

        public string SerializedTags
        {
            get
            {
                StringBuilder serializedTags = new StringBuilder();

                foreach (Tag tag in tags)
                {
                    if (serializedTags.Length == 0)
                    {
                        serializedTags.Append(tag.Name);
                    }
                    else
                    {
                        serializedTags.AppendFormat(", {0}", tag.Name);
                    }
                }

                return serializedTags.ToString();
            }
        }

        #endregion

        #region public methods

        public void RegisterTagKey(Guid key)
        {
            if (key == Guid.Empty)
            {
                throw new ArgumentException("key");
            }

            // if the key is already registered and is being updated
            // throw. Keys cannot be updated once it has been registered
            if (tagKey == Guid.Empty)
            {
                TagStore.Instance.Register(key);
            }
            else
            {
                if (tagKey != key)
                {
                    throw new InvalidOperationException("key");
                }
            }

            tagKey = key;
        }

        public void AddTag(string tag)
        {
            if (string.IsNullOrEmpty(tag))
            {
                throw new ArgumentNullException("tag");
            }

            Tag t = new Tag();
            t.Name = tag;
            t.Weight = CalculateTagWeight();

            if (!tags.Contains(t))
            {
                tags.Add(t);
                IsTagDirty = true;
            }
        }

        public void AddTags(string tags)
        {
            if (string.IsNullOrEmpty(tags))
            {
                throw new ArgumentNullException("tags");
            }

            string[] tokens = tags.Split(new char[] { SerializedTagsDelimiter }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string token in tokens)
            {
                AddTag(token);
            }
        }

        public void RemoveTag(string tag)
        {
            if (string.IsNullOrEmpty(tag))
            {
                throw new ArgumentNullException("tag");
            }

            Tag t = new Tag();
            t.Name = tag;

            tags.Remove(t);

            IsTagDirty = true;
        }

        public void RemoveTags()
        {
            tags.Clear(); 
            IsTagDirty = true;
        }

        public void InitializeTags(IList<Tag> tags)
        {
            if (tags != null)
            {
                this.tags.AddRange(tags);
            }
        }

        #endregion

        #region private methods

        private int CalculateTagWeight()
        {
            // first tag's weight i 100
            // then next 2 tags are 90
            // then next 3 tags are 80
            // rest are 50

            int w = 50;

            if (tags.Count == 0)
            {
                w = 100;
            }
            else if (tags.Count < 3)
            {
                w = 90;
            }
            else if (tags.Count < 6)
            {
                w = 80;
            }

            return w;

        }

        #endregion

    }
}
