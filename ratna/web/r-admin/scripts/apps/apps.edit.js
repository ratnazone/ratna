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
/// this javascript code is to support apps edit page
///

$(document).ready(function () {
    Setup();
});

function Setup() {

    $("#activatebtn").click(function () {
        //activate the app
        ActivateApp(true);
    });

    $("#deactivatebtn").click(function () {
        //activate the app
        ActivateApp(false);
    });

    $("#saveproperties").click(function () {
        //save app properties
        SaveProperties();
    });

    $("#deletebtn").click(function () {
        //delete app
        DeleteApp();
    });


    $("#cancelbtn").click(function () {
        OnCancelClick($(this));
    });
}

function OnCancelClick() {
    this.window.location = Constants.Apps.Url;
}

function DeleteApp() {
    // check that an app has been selected for app
    var appid = $("#appid").val();
    Notification.Show("#commonnotification", L_DeletingApp, true);

    var data = "id={0}".formatEscape(appid);
    var posturl = Constants.AppsService.DeleteUrl;

    //post back
    PostManager.post(posturl, data, OnDeleteComplete);

}

// post-submit callback
function OnDeleteComplete(responseText, statusText) {
    var success = false;
    var message = "";
    var appId = "";

    try {
        var response = jQuery.parseJSON(statusText);
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
        Notification.Show("#commonnotification", L_DeletedApp, true);
        this.window.location = Constants.Apps.Url;
    }
    else {
        message = "{0} {1}".format(L_DeletionAppFailed, message);
        Notification.Show("#commonnotification", message, false);
    }

}

function ActivateApp(enable) {
    // check that an app has been selected for app
    var appid = $("#appid").val();
    Notification.Show("#commonnotification", L_ActivatingApp, true);

    var data = "id={0}&enable={1}".formatEscape(appid, enable);
    var posturl = Constants.AppsService.ActivateUrl;

    //post back
    PostManager.post(posturl, data, OnActivationComplete);
}

// post-submit callback
function OnActivationComplete(responseText, statusText) {
    var success = false;
    var message = "";
    var appId = "";

    try {
        var response = jQuery.parseJSON(statusText);
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
        Notification.Show("#commonnotification", L_ActivatedApp, true);
    }
    else {
        message = "{0} {1}".format(L_ActivationAppFailed, message);
        Notification.Show("#commonnotification", message, false);
    }

}

function SaveProperties() {

    SavingNotification.Show();

    var data = "";
    var properties = "";

    // grab all the app properties value
    $("#fieldsInnerDiv input").each(function () {
        var input = $(this);
        if (data != "") {
            data += "&";
        }

        if (properties != "") {
            properties += ",";
        }

        data += input.attr("id") + "=" + encodeURIComponent(input.val());
        properties += input.attr("id");
    }
    );

    // add the appid
    data += "&id=" + $("#appid").val();

    // add the properties
    data += "&properties=" + properties;

    PostManager.post(Constants.AppsService.SavePropertiesUrl, data, OnSavePropertiesComplete);
}

function OnSavePropertiesComplete(responseText, statusText) {
    var success = false;
    var message = "";
    var appId = "";

    try {
        var response = jQuery.parseJSON(statusText);
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

    SavingNotification.OnSavingComplete(success, message);

}
