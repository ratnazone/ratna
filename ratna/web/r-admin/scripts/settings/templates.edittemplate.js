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
/// this javascript code is to support templates edit page
///

$(document).ready(function () {
    Setup();
});

function Setup() {

    $("#cancelbutton").click(function () {
        OnCancelClick();
    });

    $("#uploadCancelButton").click(function () {
        OnCancelClick();
    });

    $("#savebutton").click(function () {
        //OnSaveClick();
    });

    $("#uploadTemplateButton").click(function () {
        //upload the selected template
        uploadTemplate();
    });

    SetValidation();
}

function SetValidation() {

    $.validator.setDefaults({
        submitHandler: function () {
            OnSaveClick();
            return false;
        }
    });

    $("#template_form").validate();

}

function OnCancelClick() {
    this.window.location = Constants.Settings.Url;
}


function OnSaveClick() {

    try {
        var urlPath = $("#urlPath").val();
        var name = $("#name").val();
        var templatePath = $("#templatePath").val();
        var masterFile = $("#masterFile").val();
        var uid = $("#uid").val();
        var isActive = false;


        if ($("#activatedTemplate").is(":checked")) {
            isActive = true;
        }

        var data = "uid={0}&name={1}&url={2}&path={3}&master={4}&active={5}".format(uid, name, urlPath, templatePath, masterFile, isActive);

        SavingNotification.Show();
        PostManager.post(Constants.TemplatesService.SaveTemplateUrl, data, OnSaveComplete, true /* json */);
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

    SavingNotification.OnSavingComplete(response.success, message);
}


// add the template uploader to link
$(function () {
    var theForm = $('#theForm');
    $('#templateselector').file().choose(function (e, input) {

        // remove any file that already has been selected
        theForm.find("[type=file]").remove();

        var success = displaySelectedTemplate(input);
        $(input).hide();

        if (success) {
            theForm.append(input);
        }

    });

    $("#templatename").focus(function () {
        var t = $("#templatename");
        if (t != null && t != "") {
            $("#templatename").removeClass("errorValidate");
        }
    });

    $("#templateurlpath").focus(function () {
        var t = $("#templateurlpath");
        if (t != null && t != "") {
            $("#templateurlpath").removeClass("errorValidate");
        }
    });
    
});

function displaySelectedTemplate(input) {
    var flag = false;

    if (input != null) {

        var filePath = input.val();
        var dotIndex = filePath.lastIndexOf('.');

        if (dotIndex > 0) {
            var slashIndex = filePath.lastIndexOf('\\');

            if (slashIndex == -1) {
                slashIndex = 0;
            }

            var filename = filePath.substring(slashIndex + 1);
             
            var ext = filePath.substring(dotIndex + 1);
            ext = ext.toLowerCase();

            if (ext == "zip") {
                flag = true;

                //set the display to show which file was selected
                $("#selectedTemplate").text(filename);
            }
        }

    }

    if (!flag) {
        Notification.Show("#notification", L_TemplateUploadMustBeZip, false)
    }

    return flag;
}

function uploadTemplate() {
    //make sure every entry in entered
    var name = $("#templatename").val();
    var urlPath = $("#templateurlpath").val();

    if (name == null || name == "") {
        $("#templatename").addClass("errorValidate");
    }
    else if (urlPath == null || urlPath == "") {
        $("#templateurlpath").addClass("errorValidate");
    }
    else {
        $("#uploadtemplatename").val(name);
        $("#uploadtemplateurlPath").val(urlPath);
        SavingNotification.Show();
        var options = { success: showUploadResponse };
        $("#theForm").ajaxSubmit(options);
    }
}

// post-submit callback
function showUploadResponse(responseText, statusText) {
    var success = false;
    var message = "";
    var templateId = "";

    try {
        var response = jQuery.parseJSON(responseText);
        if (response.success) {
            success = true;
            templateId = response.Id;
        }
        else {
            message = response.error;
        }
    }
    catch (err) {
        message = err;
    }

    SavingNotification.OnSavingComplete(success, message);

    if (success) {
        // redirect the page to load the template editing mode.
        window.location = Constants.Templates.EditUrlWithKey + templateId;
    }
}
