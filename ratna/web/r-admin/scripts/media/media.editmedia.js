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
//
// media.editmedia.js
//

$(document).ready(function () {
    SetValidation();
    Setup();
});

function Setup() {

    $("#cancelbutton").click(function () {
        OnCancelClick($(this));
    });

    $("#deletemedia").click(function () {
        OnDeleteClick();
    });

}

function SetValidation() {

    $.validator.setDefaults({
        submitHandler: function () {
            OnSaveClick();
            return false;
        }
    });

    $("#media_form").validate();

}

// file uploader
$(function () {
    var $theForm = $('#theForm');
    $('#mediauploader').file().choose(function (e, input) {
        $(input).hide();
        $theForm.append(input);

        //submit the form
        uploadMedia();
    });
});

function OnSaveClick() {
    var url = $("#urlInput").val();
    var name = $("#nameInput").val();
    var tags = $("#tagsInput").val();
    var mediaType = $("#uploadtype").val();

    if (tags == null) {
        tags = "";
    }

    var data = "url={0}&name={1}&tags={2}&mediaType={3}".formatEscape(url, name, tags, mediaType);
    var posturl = Constants.MediaService.SaveUrl;

    SavingNotification.Show();
    PostManager.post(posturl, data, OnSaveComplete);
}

///
/// OnSaveComplete - called when the save is completed for the photo
///
function OnSaveComplete(success, data) {
    var message = "";
    var success = false;

    try {
        var response = jQuery.parseJSON(data);

        if (!response.success) {
            message = response.error;
        }
        else {

            // save is completed.
            $("#photoimage").attr("src", response.location);

            // update the width and height
            $("#heightspan").text(response.height);
            $("#widthspan").text(response.width);

            // photo cannot be uploaded anymore
            $("#mediauploader").hide();

            $("#headerH1").text(L_EditMediaHeader);
            success = true;
        }
    }
    catch (err) {
        message = err;
    }

    SavingNotification.OnSavingComplete(success, message);
}

function OnCancelClick() {
    this.window.location = Constants.Media.Url;
}

function OnDeleteClick() {
    var url = $("#urlInput").val();

    var data = "url={0}".formatEscape(url);
    var posturl = Constants.MediaService.DeleteUrl;

    SavingNotification.Show();
    PostManager.post(posturl, data, OnDeleteComplete);
}

///
/// OnDeleteComplete - called when the delete is completed for the photo
///
function OnDeleteComplete(success, data) {
    var response = jQuery.parseJSON(data);
    var message = "";

    if (!response.success) {
        message = response.error;
    }
    else {
        this.window.location = Constants.Media.Url;
    }
}

function uploadMedia() {
    SavingNotification.Show();
    var options = { success: showMediaUploadResponse, cache: false };
    $("#uploadtype").attr("name", "uploadtype");
    $("#theForm").ajaxSubmit(options);
}

// post-submit callback for media upload
function showMediaUploadResponse(responseText, statusText) {
    var success = false;
    var message = "";

    try {
        var response = jQuery.parseJSON(responseText);
        if (response.success) {
            success = true;
            $("#photoimage").attr("src", response.location);

            //set the other variables
            $("#urlInput").val(response.location);
            $("#heightspan").text(response.height);
            $("#widthspan").text(response.width);

            $("#nameInput").val(response.name);

            // once the media has uploaded
            // url cannot be changed
            $("#urlInput").attr("disabled", "disabled");

            //fix the breadcrumb
            BreadCrumb.SetLastAnchorHref(Constants.Media.EditUrlWithKey + response.location + "&view=" + $("#uploadtype").val());
        }
        else {
            message = response.error;
        }
    }
    catch (err) {
    }

    //incase of error, flush the form and remove the file.
    if (!success) {
        $('#theForm input').each(function () {
            var inputfile = $(this);
            if (inputfile.attr("type") == "file") {
                inputfile.remove();
            }
        });
    }

    SavingNotification.OnSavingComplete(success, message);
}
