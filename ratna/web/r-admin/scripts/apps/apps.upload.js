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
/// this javascript code is to support apps upload from local box
///

$(document).ready(function () {
    Setup();
});

function Setup() {

    $("#uploadAppButton").click(function () {
        //upload the selected app
        uploadApp();
    });

}


// add the template uploader to link
$(function () {
    var theForm = $('#theForm');
    $('#appselector').file().choose(function (e, input) {

        // remove any file that already has been selected
        theForm.find("[type=file]").remove();

        var success = displaySelectedApp(input);
        $(input).hide();

        if (success) {
            theForm.append(input);
        }

    });
    
});

function displaySelectedApp(input) {
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
                $("#selectedApp").text(filename);
            }
        }

    }

    if (!flag) {
        Notification.Show("#notification", L_AppUploadMustBeZip, false)
    }

    return flag;
}

function uploadApp() {
    // check that an app has been selected for app
    var app = $("#selectedApp").text();

    if (app == null || app == "") {
        Notification.Show("#notification", L_SelectAnAppToUpload, false)
    }
    else {
        SavingNotification.Show();
        var options = { success: showUploadResponse };
        $("#theForm").ajaxSubmit(options);
    }
}

// post-submit callback
function showUploadResponse(responseText, statusText) {
    var success = false;
    var message = "";
    var appId = "";

    try {
        var response = jQuery.parseJSON(responseText);
        if (response.success) {
            success = true;
            appId = response.Id;
        }
        else {
            message = response.error;
        }
    }
    catch (err) {
        message = err;
    }

    if (success) {
        // load the page to edit the app.
        window.location.href = "{0}{1}".format(Constants.Apps.EditUrlWithKey, appId);
    }
    else {
        SavingNotification.OnSavingComplete(success, message);
    }
}
