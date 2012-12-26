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
/// javascript code for Configuration.ascx
$(document).ready(function () {
    NotificationSettingsSetup();
});

function NotificationSettingsSetup() {

    $("#smtpSettingCancelButton").click(function () {
        OnCancelClick();
    });

    $("#notificationCancelButton").click(function () {
        OnCancelClick();
    });

    $("#smtpSettingSaveButton").click(function () {
        OnSmtpConfigurationSaveButtonClick($(this));
    });

    $("#notificationSaveButton").click(function () {
        OnNotificationSaveButtonClick($(this));
    });
}


function OnSmtpConfigurationSaveButtonClick(saveButton) {

    var smtpAddress = $("#smtpaddress").val();
    var smtpUserName = $("#smtpusername").val();
    var smtpPassword = $("#smtppassword").val();
    var smtpFromEmail = $("#fromemail").val();

    var data = "smtpAddress={0}&smtpUserName={1}&smtpPassword={2}&smtpFrom={3}".format(smtpAddress, smtpUserName, smtpPassword, smtpFromEmail);

    PostManager.post(Constants.SiteConfigurationService.UpdateSmtpUrl, data, OnSmtpConfigurationSaveCompleted);

}

function OnNotificationSaveButtonClick(saveButton) {

    var notifyTo = $("#notifyto").val();
    var comment = $("#commentNotificationSelect").val();
    var formsResponse = $("#formsResponseNotificationSelect").val();

    var data = "notificationEmail={0}&comment={1}&formsResponse={2}".format(notifyTo, comment, formsResponse);

    PostManager.post(Constants.SiteConfigurationService.UpdateNotificationUrl, data, OnSmtpConfigurationSaveCompleted);

}

function OnSmtpConfigurationSaveCompleted(success, data) {
 
    var response = jQuery.parseJSON(data);
    var message = "";

    if (!response.success) {
        message = response.error;
    }
    else {
        // save has been successful.
        message = L_SavedSuccees;
    }

    Notification.Show("#cnotification", message, response.success);
}

function OnCancelClick() {
    window.location = CancelUrl;
}
