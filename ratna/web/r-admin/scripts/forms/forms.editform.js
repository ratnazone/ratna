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
///
/// this javascript code is to support editing of forms
///

$.validator.setDefaults({
    submitHandler: function (form) {
        OnSaveClick();
        return false;
    }
});

$(document).ready(function () {
    $("#editform").validate();


    $("#cancelbutton").click(function () {
        OnCancelClick();
    });

    $("#addNewField").click(function () {
        OnAddNewFieldClick();
    });

    $("#newfieldname").keypress(function () {
        $("#newfieldname").removeClass("errorValidate");
    });

    // add delete to every delete button
    $(".deleteFieldImageButton").click(function () {
        OnDeleteFieldButtonClick($(this));
    });

});

function OnDeleteFieldButtonClick(deleteButton) {
    if (deleteButton != null) {
        var tr = deleteButton.parent().parent();
        var fieldNameInput = tr.find("#fieldname")
        var fieldname = fieldNameInput.val();
        var formname = $("#name").val();

        ConfirmDialog.confirm(
            L_DeleteFieldConfirmation.format(fieldname), function () {
                OnDeleteFieldConfirmed(formname, fieldname);
            }
        );
    }
}

function OnDeleteFieldConfirmed(formname, fieldname) {
    var data = "formname={0}&fieldname={1}".formatEscape(formname, fieldname);
    PostManager.post(Constants.FormsManageService.DeleteFieldUrl, data, OnDeleteFieldCompleted, fieldname);
}

function OnDeleteFieldCompleted(success, data, fieldname) {
    var response = jQuery.parseJSON(data);
    if (response.success) {

        var fieldNameInputField = $('input[value="' + fieldname + '"]');

        //get the parent's parent
        var tr = fieldNameInputField.parent().parent();

        // remove the tr
        tr.remove();

        ShowDeleteFieldStatus(L_DeleteFieldSuccess, true /* autohide */);
    }
    else {
        ShowDeleteFieldStatus(response.error, false /* autohide */);
    }
}

function ShowDeleteFieldStatus(message, autohide) {
    Notification.Show("#commonnotification", message, autohide)
}

function OnCancelClick() {
    this.window.location = Constants.Forms.Url;
}

function OnSaveClick() {

    try {
        // save the form
        var formname = $("#name").val();
        var displayname = $("#displayname").val();
        var uid = $("#uid").val();

        var data = "uid={0}&formname={1}&displayname={2}".formatEscape(uid, formname, displayname);
        SavingNotification.Show();
        PostManager.post(Constants.FormsManageService.SaveFormUrl, data, OnSaveComplete);
    }
    finally {

        return false;
    }
}

///
/// OnSaveComplete - called when the save is completed for the template
///
function OnSaveComplete(success, data) {
    var response = jQuery.parseJSON(data);
    var message = "";

    if (!response.success) {
        message = response.error;
    }
    else {
        // update the uid
        $("#uid").val(response.uid);

        //update the breadcrumb.
        var formname = $("#name").val();
        BreadCrumb.SetLastAnchorHref(Constants.Forms.EditUrl.format(formname));
    }

    SavingNotification.OnSavingComplete(response.success, message);
}

function OnAddNewFieldClick() {

    var formname = $("#name").val();
    var name = $("#newfieldname").val();
    var type = $("#newfieldselect").val();
    var reqd = $("#newfieldrequired:checked").val();

    if (name == null || name == "") {
        $("#newfieldname").addClass("errorValidate");
        return false;
    }

    // make sure that the field names are not duplicated


    if (reqd == null) {
        reqd = "false";
    }
    else {
        reqd = "true";
    }

    // get the snippet with addition of the field
    var snippetManager = new SnippetManager();
    snippetManager.enable("FormFieldRowControl");
    snippetManager.add("FieldName");
    snippetManager.add("FieldType");
    snippetManager.add("Required");

    data = "formname={0}&fieldname={1}&fieldtype={2}&required={3}".formatEscape(formname, name, type, reqd);
    data = "{0}&{1}".format(data, snippetManager.getQueryString());

    PostManager.post(Constants.FormsManageService.AddFieldUrl, data, OnAddNewFieldCompleted);
}

function OnAddNewFieldCompleted(success, data) {
    var response = jQuery.parseJSON(data);
    if (response.success) {

        // add the html infront of the aanew
        $(response.html).insertBefore($("#addnewfieldtr"));

        ResetAddNewFieldForm();
    }
    else {
        Notification.Show("#commonnotification", response.error, false /*autohide*/)
    }
}

function ResetAddNewFieldForm() {
    $("#newfieldname").val("");
    $("#newfieldselect").val("String");
    $("#newfieldrequired").attr("checked", "false");
}
