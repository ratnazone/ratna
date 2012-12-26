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
namespace Jardalu.Ratna.Core.Forms
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

    public class FormsPlugin : SystemPlugin
    {
        #region private fields

        private static FormsPlugin instance = null;
        private static object syncRoot = new object();

        #endregion

        #region ctor

        public FormsPlugin()
        {
            this.Name = "Forms";
            this.Id = new Guid("ffa99fec-29fb-42ac-a0e9-db189e543573");

            this.Register();
            this.Activate();
        }

        #endregion

        #region public properties

        public static FormsPlugin Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                        {
                            instance = new FormsPlugin();
                            instance.Register();
                            instance.Activate();
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

        public bool TryRead(string name, out Form form)
        {
            form = null;
            bool success = false;

            try
            {
                form = PluginStore.Instance.Read<Form>(this, Form.KeyName, name);
                success = true;
            }
            catch (MessageException me)
            {
                if (me.ErrorNumber != PluginErrorCodes.PluginDataNotFound)
                {
                    throw;
                }
            }

            return success;
        }

        public Form Read(string name)
        {
            return PluginStore.Instance.Read<Form>(this, Form.KeyName, name);
        }

        public void Delete(string name)
        {
            PluginStore.Instance.Delete(this, Form.KeyName, name);
        }

        public void Add(Form form)
        {
            #region argument checking

            if (form == null)
            {
                throw new ArgumentNullException("form");
            }

            if (!form.IsValid())
            {
                throw new InvalidOperationException("form is invalid");
            }

            #endregion

            try
            {
                // add the form
                PluginStore.Instance.Save(this, form);
            }
            catch (MessageException me)
            {
                if (me.ErrorNumber == PluginErrorCodes.IdAlreadyInUse)
                {
                    // throw with a new message
                    throw new MessageException(me.ErrorNumber, ResourceManager.GetMessage(FormMessages.NameAlreadyInUse));
                }

                throw;
            }
        }

        public IList<Form> GetForms(string query, int start, int count, out int total)
        {
            if ((start < 0) || (count < 0))
            {
                throw new ArgumentException("start or count");
            }

            PluginDataQueryParameter parameter = null;

            if (!string.IsNullOrEmpty(query))
            {
                parameter = new PluginDataQueryParameter("name", query);
            }

            return PluginStore.Instance.Read<Form>(this, parameter, start, count, out total);

        }

        public IList<Form> GetFormsEntryAddedAfter(DateTime after)
        {
            Dictionary<string, Form> formsDictionary = new Dictionary<string, Form>();

            // get all the entries added after the datetime
            IList<FormEntry> entries = FormEntryPlugin.Instance.GetFormResponses(after);

            if (entries != null && entries.Count > 0)
            {
                foreach (FormEntry entry in entries)
                {
                    if (!formsDictionary.ContainsKey(entry.Key))
                    {
                        Form form;

                        // load the form.
                        if (FormsPlugin.Instance.TryRead(entry.Key, out form))
                        {
                            formsDictionary[form.Name] = form;
                        }

                    }
                }
            }

            IList<Form> forms = new List<Form>(formsDictionary.Count);

            foreach (Form f in formsDictionary.Values)
            {
                forms.Add(f);
            }

            return forms;
        }

        #endregion

    }

}
