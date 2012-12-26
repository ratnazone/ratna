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
// gallery.edit.js
//

$(document).ready(function () {
    SetValidation();
    Setup();
});

function Setup() {

    $("#cancelbutton").click(function () {
        OnCancelClick($(this));
    });

    $("#gallerySelectButton").ImagePicker({
        'searchUrl': Constants.MediaService.SearchUrl
    }, OnGalleryImagesSelected);

}

function SetValidation() {

    $.validator.setDefaults({
        submitHandler: function () {
            OnSaveClick();
            return false;
        }
    });

    $("#editform").validate();

}

function OnGalleryImagesSelected(images) {
    if (images != null && images.length > 0) {

        //photos is an array of string
        var photos = "";
        for (var i = 0; i < images.length; i++) {
            photos += "photos=" + images[i];

            if (i != images.length - 1) {
                photos += "&";
            }
        }

        var url = $("#urlInput").val();

        // add these images to the gallery
        var data = "url={0}&{1}".format(url, photos);
        var posturl = Constants.GalleryService.AddPhotoUrl;

        SavingNotification.Show();
        PostManager.post(posturl, data, OnGalleryImagesAddedComplete, images);        
    }
}

// images has been added to gallery, update the UI
function OnGalleryImagesAddedComplete(success, data, images) {

    var message = "";
    var osuccess = false;

    try {
        var response = jQuery.parseJSON(data);

        if (!response.success) {
            message = response.error;
        }
        else {
            osuccess = true;
            if (images != null && images.length > 0) {
                // display images to the page.
                for (var i = 0; i < images.length; i++) {
                    imageHtml = "<img style='width:120px;height:120px;margin-top:10px;margin-right:10px;border:2px solid silver;' src='" + images[i] + "'/>";
                    $(imageHtml).insertBefore("#galleryAddedImagesMarker");
                }
            }
        }
    }
    catch (err) {
        message = err;
    }

    SavingNotification.OnSavingComplete(osuccess, message);
    
}


function OnSaveClick() {
    var url = $("#urlInput").val();
    var name = $("#name").val();
    var description = $("#description").val();
    var uid = $("#uid").val();
    var nav = $("#nav").val();

    var data = "url={0}&name={1}&description={2}&nav={3}&uid={4}".formatEscape(url, name, description, nav, uid);
    var posturl = Constants.GalleryService.SaveUrl;

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
            success = true;
            $("#headerH1").text(L_EditGalleryHeader);

            var url = $("#urlInput").val();
            //update the bread crumb.
            BreadCrumb.SetLastAnchorHref(Constants.Media.GalleryEditUrlWithKey.format(url));
        }
    }
    catch (err) {
        message = err;
    }

    SavingNotification.OnSavingComplete(success, message);
}

function OnCancelClick() {
    this.window.location = Constants.Gallery.Url;
}


