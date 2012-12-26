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
namespace Jardalu.Ratna.Web.Admin.service
{
    #region using

    using System;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    using System.Web;
    using System.Web.Script.Services;
    using System.Web.Services;

    using Jardalu.Ratna.Core;
    using Jardalu.Ratna.Core.Apps;
    using Jardalu.Ratna.Core.Forms;
    using Jardalu.Ratna.Exceptions;
    using Jardalu.Ratna.Exceptions.ErrorCodes;
    using RatnaUser = Jardalu.Ratna.Profile.User;
    using Jardalu.Ratna.Store;
    using Jardalu.Ratna.Utilities;
    using Jardalu.Ratna.Web.Service;    
    using Jardalu.Ratna.Web.UI.Snippets;    
    using Jardalu.Ratna.Web.Resource;
    using Jardalu.Ratna.Web.Utilities;
    using Jardalu.Ratna.Web.Applications;
    
    #endregion

    [Access(Group = "Administrator")]
    [ScriptService]
    public class formsmanage : ServiceBase
    {

        #region private fields

        private static Logger logger = Logger.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #endregion


        [WebMethod]
        [ScriptMethod(UseHttpGet = true)]
        [SupportedSnippet("FormFieldRowControl")]
        public string AddField(string formname, string fieldname, string fieldtype, string required)
        {
            RatnaUser user = base.ValidatedUser();
            if (user == null)
            {
                return SendAccessDenied();
            }

            ServiceOutput output = new ServiceOutput();
            output.Success = false;

            bool validFieldName = true;

            if (string.IsNullOrEmpty(fieldname) ||
                !Regex.IsMatch(fieldname, "^[0-9a-zA-Z]+$"))
            {
                // field name with space and alpha numbers
                validFieldName = false;
            }

            if (validFieldName)
            {
                #region field name is valid

                FieldType ft = FieldType.Other;

                if (Enum.TryParse<FieldType>(fieldtype, true, out ft))
                {
                    bool isRequired = false;
                    bool success = false;

                    if (!Boolean.TryParse(required, out isRequired))
                    {
                        isRequired = false;
                    }

                    Form form = null;

                    if (!string.IsNullOrEmpty(formname))
                    {
                        FormsPlugin.Instance.TryRead(formname, out form);
                    }

                    if (form != null)
                    {
                        // if the field already exists, error out.
                        if (form.Fields.Contains(new Field() { Name = fieldname }))
                        {
                            output.AddOutput(Constants.Json.Error,
                                ResourceManager.GetLiteral("Admin.Forms.Field.Save.Error.FieldNameInUse"));
                        }
                        else
                        {
                            //add the field to the form
                            form.AddField(fieldname, ft, isRequired);
                            FormsPlugin.Instance.Save(form);
                            success = true;
                        }

                    }

                    if (success)
                    {
                        this.AddSnippet(output, System.Reflection.MethodBase.GetCurrentMethod());
                        output.Success = true;
                    }
                }

                #endregion
            }
            else
            {
                output.AddOutput(Constants.Json.Error,
                    ResourceManager.GetLiteral("Admin.Forms.Field.Save.Error.FieldNameInInvalid"));
            }

            return output.GetJson();
        }

        [WebMethod]
        [ScriptMethod(UseHttpGet = true)]
        public string DeleteField(string formname, string fieldname)
        {

            RatnaUser user = base.ValidatedUser();
            if (user == null)
            {
                return SendAccessDenied();
            }

            ServiceOutput output = new ServiceOutput();
            output.Success = false;

            if (!string.IsNullOrEmpty(formname) && !string.IsNullOrEmpty(fieldname))
            {
                Form form = null;
                FormsPlugin.Instance.TryRead(formname, out form);
                
                if (form != null)
                {
                    form.RemoveField(fieldname);
                    FormsPlugin.Instance.Save(form);
                    output.Success = true;
                }
                else
                {
                    // no form or field found.
                    output.AddOutput(Constants.Json.Error, 
                                string.Format(ResourceManager.GetLiteral("Admin.Forms.FormOrFieldNotFound"), formname, fieldname)
                            );
                }
            }

            return output.GetJson();
        }

        [WebMethod]
        [ScriptMethod(UseHttpGet = true)]
        public string Save(string uid, string formname, string displayname)
        {
            RatnaUser user = base.ValidatedUser();
            if (user == null)
            {
                return SendAccessDenied();
            }

            ServiceOutput output = new ServiceOutput();
            output.Success = false;

            // without the formname and displayname, it cannot be saved.
            if (string.IsNullOrEmpty(formname)
                || string.IsNullOrEmpty(displayname))
            {
                // do nothing.
                output.AddOutput(Constants.Json.Error, ResourceManager.GetLiteral("Admin.Forms.Save.Error.NameOrDisplayNameNotSpecified"));
            }
            else
            {

                try
                {
                    // convert the uid to GUID. if failed to convert, assume that this is a new form.
                    Guid uuid;
                    if (Guid.TryParse(uid, out uuid))
                    {
                        Form form;
                        if (FormsPlugin.Instance.TryRead(formname, out form))
                        {
                            // match the uid
                            if (uuid == form.UId)
                            {
                                logger.Log(LogLevel.Info, "Updating form - {0}, uid - {1}", formname, uuid);
                                form.DisplayName = displayname;

                                FormsPlugin.Instance.Add(form);
                                output.AddOutput("uid", form.UId);
                                output.Success = true;
                            }
                            else
                            {
                                logger.Log(LogLevel.Warn, "Unble to save form with name - [{0}] and uid - {1}. Uid and name mismatch", formname, uuid);

                                // mismatch of name and uid
                                output.AddOutput(Constants.Json.Error, ResourceManager.GetLiteral("Admin.Forms.Save.Error.NameAndUIdMismatch"));
                            }
                        }
                        else
                        {
                            // unable to locate form with the given name
                            logger.Log(LogLevel.Warn, "Unble to locate form with name - [{0}] and uid - {1}", formname, uuid);
                            output.AddOutput(Constants.Json.Error, ResourceManager.GetLiteral("Admin.Forms.Save.Error.NoFormFoundWithName"));
                        }
                    }
                    else
                    {
                        logger.Log(LogLevel.Info, "Creating new form - {0}, display name - [{1}]", formname, displayname);

                        Form form = new Form();
                        try
                        {
                            form.Name = formname;
                        }
                        catch (ArgumentException)
                        {
                            throw new MessageException(ResourceManager.GetLiteral("Admin.Forms.Save.Error.FormNameInvalid"));
                        }

                        form.DisplayName = displayname;

                        FormsPlugin.Instance.Add(form);

                        output.AddOutput("uid", form.UId);
                        output.Success = true;
                    }
                }
                catch (MessageException me)
                {
                    logger.Log(LogLevel.Warn, "Unble to save form - {0} ", me);

                    if (me.ErrorNumber == PluginErrorCodes.IdAlreadyInUse)
                    {
                        output.AddOutput(Constants.Json.Error, ResourceManager.GetLiteral("Admin.Forms.Save.Error.FormNameInUse"));
                    }
                    else
                    {
                        output.AddOutput(Constants.Json.Error, me.Message);
                    }

                    output.Success = false;
                }
            }

            return output.GetJson();
        }

        [WebMethod]
        [ScriptMethod(UseHttpGet = true)]
        public string Delete(string formname)
        {
            RatnaUser user = base.ValidatedUser();
            if (user == null)
            {
                return SendAccessDenied();
            }

            ServiceOutput output = new ServiceOutput();
            output.Success = false;

            // without the formname it cannot be deleted
            if (!string.IsNullOrEmpty(formname))
            {
                logger.Log(LogLevel.Info, "Deleting form - {0}", formname);

                FormsPlugin.Instance.Delete(formname);
                output.Success = true;
            }

            return output.GetJson();
        }

        [WebMethod]
        [ScriptMethod(UseHttpGet = true)]
        public string DeleteResponses(string formname, string uids)
        {
            RatnaUser user = base.ValidatedUser();
            if (user == null)
            {
                return SendAccessDenied();
            }

            ServiceOutput output = new ServiceOutput();
            output.Success = false;

            // without the formname responses cannot be deleted
            if (!string.IsNullOrEmpty(formname) &&
                !string.IsNullOrEmpty(uids))
            {
                logger.Log(LogLevel.Info, "Deleting responses form - {0}, uids - {1}", formname, uids);

                string[] tokens = uids.Split(',');
                List<Guid> uidsList = new List<Guid>();
                foreach (string token in tokens)
                {
                    Guid guid;
                    if (Guid.TryParse(token, out guid))
                    {
                        uidsList.Add(guid);
                    }
                }

                // call delete on responses
                FormEntryPlugin.Instance.Delete(uidsList);
                
                output.Success = true;
            }

            return output.GetJson();
        }

        [WebMethod]
        [ScriptMethod(UseHttpGet = true)]
        public string EditEntry(string form, string uid, string fields)
        {
            //make sure the user has access
            RatnaUser user = base.ValidatedUser();
            if (!(IsAccessAllowed(user)))
            {
                return SendAccessDenied();
            }

            logger.Log(LogLevel.Debug, "EditEntry called for form - [{0}] - uid [{1}].", form, uid);

            ServiceOutput output = new ServiceOutput();
            output.Success = false;

            Guid guid = Guid.Empty;
            if (Guid.TryParse(uid, out guid))
            {
                AddFormEntry(form, guid, fields, output);
            }

            return output.GetJson();
        }

        #region internal methods

        internal static void AddFormEntry(string formname, Guid uid, string fields, ServiceOutput output)
        {
            if (!string.IsNullOrEmpty(formname) &&
                !string.IsNullOrEmpty(fields))
            {
                //get the form.
                Form form = null;
                if (FormsPlugin.Instance.TryRead(formname, out form))
                {
                    //form was found.
                    logger.Log(LogLevel.Info, "Attempting to save entry for form - {0}.", form.Name);

                    //check if there is already an entry with uid
                    FormEntry entry = null;

                    if (uid != Guid.Empty)
                    {
                        // get the entry
                        entry = PluginStore.Instance.Read<FormEntry>(FormEntryPlugin.Instance, uid);
                    }

                    if (entry == null)
                    {
                        //create a new entry
                        entry = new FormEntry();
                        entry.Form = formname;
                        entry.Id = Utility.GetUniqueString();
                    }

                    // split the fields
                    string[] tokens = fields.Split(',');
                    foreach (string field in tokens)
                    {
                        if (string.IsNullOrEmpty(field))
                        {
                            continue;
                        }

                        //generate the response data
                        Data data = new Data();
                        data.Name = field;
                        data.Value = HttpContext.Current.Request[field];

                        entry.Add(data);
                    }

                    try
                    {
                        // app execution before saving the entry
                        AppEngine.ExecuteApps(AppEvent.FormEntrySaving, entry);

                        // submit the response
                        FormEntryPlugin.Instance.Add(entry);

                        AppEngine.ExecuteApps(AppEvent.FormEntrySaved, entry);

                        //notify
                        Notifier.Notify(ResourceManager.GetLiteral("FormResponses.NewResponseSubject") /* subject */,
                                        string.Format(ResourceManager.GetLiteral("FormResponses.NewResponseBody"), form.DisplayName) /* body */);

                        output.Success = true;
                        output.AddOutput("uid", entry.UId);
                    }
                    catch (MessageException me)
                    {
                        string errorMessage = me.Message;
                        if (me.ErrorNumber == FormsErrorCodes.NotAllRequiredFieldsSupplied)
                        {
                            errorMessage = ResourceManager.GetLiteral("FormResponses.NotAllRequiredFieldsSupplied");
                        }
                        else if (me.ErrorNumber == FormsErrorCodes.FieldValueDoesnotMatchWithFieldType)
                        {
                            errorMessage = ResourceManager.GetLiteral("FormResponses.FieldValueDoesnotMatchWithFieldType");
                        }

                        output.AddOutput(Constants.Json.Error, errorMessage);
                    }

                }
                else
                {
                    //form not found error
                    output.AddOutput(
                            Constants.Json.Error,
                            string.Format(ResourceManager.GetLiteral("Admin.Forms.NotFound"), formname)
                        );
                }
            }
            else
            {
                //either the form name was null, or the fields were null.
                output.AddOutput(Constants.Json.Error, ResourceManager.GetLiteral("FormResponses.Invalid"));
            }
        }

        #endregion

    }
}
