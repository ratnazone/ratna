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
$(document).ready(function () {
    Setup();
});

function Setup() {
    $("#cancelbutton").click(function () {
        OnCancelClick();
    });

    $("#savebutton").click(function () {
        OnSaveClick();
    });
}

function OnCancelClick() {
    window.location.href = CancelUrl;
}

function OnSaveClick() {
    var displayName = $("#displayname").val();
    var firstName = $("#firstname").val();
    var lastName = $("#lastname").val();
    var desc = $("#description").val();

    var data = "displayName={0}&firstname={1}&lastname={2}&description={3}".format(displayName, firstName, lastName, desc);

    SavingNotification.Show();
    PostManager.post(Constants.ProfileService.UpdateUrl, data, OnSavingComplete);

}


// file uploader
$(function () {
    var $theForm = $('#theForm');
    $('#photoselector').file().choose(function (e, input) {
        $(input).hide();
        $theForm.append(input);

        //submit the form
        uploadProfilePhoto();
    });
});

function uploadProfilePhoto() {
    SavingNotification.Show();
    var options = { success: showResponse };
    $("#theForm").ajaxSubmit(options);
}

// post-submit callback
function showResponse(responseText, statusText) {
    var success = false;
    var message = "";

    try {
        var response = jQuery.parseJSON(responseText);
        if (response.success) {
            success = true;
            $("#profileimage").attr("src", response.location);
        }
        else {
            message = response.error;
        }
    }
    catch (err) {
        success = false;
    }

    SavingNotification.OnSavingComplete(success, message);
}

function OnSavingComplete(success, data) {
    var response = jQuery.parseJSON(data);
    var message = "";

    if (!response.success) {
        message = response.error;
    }
    else {
        //success
    }

    SavingNotification.OnSavingComplete(response.success, message);
}
