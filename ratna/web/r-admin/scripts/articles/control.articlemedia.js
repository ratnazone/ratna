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
/// this javascript code is to support articles' edit page
///

$(document).ready(function () {
    Setup();
});

function Setup() {
    
    $("#photouploader").file().choose(function (e, input) {
        OnPhotoUploaderClick(input);
    });

    $("#selectallcheckbox").click(function () {
        SelectAllCheckboxClicked();
    });

    $("#deletemediaphotobutton").click(function () {
        DeleteMediaPhotoButtonClicked();
    });

    // photo selected - photoselected
    $("#photoselected").ImagePicker({
        'searchUrl': Constants.MediaService.SearchUrl
    }, OnArticleImagesSelected);

}

function SelectAllCheckboxClicked() {
    var ischecked = $("#selectallcheckbox").is(":checked");

    if (ischecked) {
        $(".addimagecheckbox").attr("checked", true);
    }
    else {
        $(".addimagecheckbox").attr("checked", false);
    }
}

function OnPhotoUploaderClick(input) {
    var $theForm = $("#theForm");
    $(input).hide();
    $theForm.append(input);

    //submit the form
    UploadArticleImage();
}

function UploadArticleImage() {
    SavingNotification.Show();
    var options = { success: OnImageUploadCompleted, cache: false };
    $("#theForm").ajaxSubmit(options);
}

// post-submit callback
function OnImageUploadCompleted(responseText, statusText) {
    var success = false;
    var message = "";

    try {
        var response = jQuery.parseJSON(responseText);
        if (response.success) {
            success = true;
            DisplayAddedImage(response.location);
        }
    }
    catch (err) {
        message = err;
    }

    // clear the form so that new files can be uploaded
    $('#theForm input').each(function () {
        var inputfile = $(this);
        if (inputfile.attr("type") == "file") {
            inputfile.remove();
        }
    });

    SavingNotification.OnSavingComplete(success, message);
}


function DisplayAddedImage(location) {
    var newAddedImageDiv = $("#addimagediv").clone();
    var newlyAddedImage = newAddedImageDiv.find("#addimage");
    newlyAddedImage.attr("src", location);
    newlyAddedImage.attr("title", "");
    
    //display the div
    newAddedImageDiv.css("display", "inline-block");

    //add to display.
    newAddedImageDiv.insertBefore("#photomarker");
}

function DeleteMediaPhotoButtonClicked() {
    var selectedImages = new Array();

    $(document).find("input.addimagecheckbox:checkbox").each(function () {
        var checkbox = $(this);

        if (checkbox.is(":checked")) {
            var img = checkbox.prev();
            var src = img.attr("src");

            if (src != "") {
                selectedImages[selectedImages.length] = src;
            }
        }
    });

    if (selectedImages.length == 0) {
        Notification.Show("#commonnotification", L_SelectArticleMediaToDelete, false /* autohide */);
        return;
    }

    // confirm selected responses
    ConfirmDialog.confirm(
            L_RemoveMultiMediaConfirmation, function () {
                OnMultipleMediaButtonClickConfirmed(selectedImages);
            }
        );
}

function OnMultipleMediaButtonClickConfirmed(selectedImages) {

    var selectedurls = "";
    var urlkey = $("#theForm").find("#urlkey").val();

    for (var i = 0; i < selectedImages.length; i++) {
        var url = selectedImages[i];
        if (selectedurls == "") {
            selectedurls += url;
        }
        else {
            selectedurls += ',' + url;
        }
    }

    var data = "urlkey={0}&images={1}".format(urlkey, selectedurls);
    PostManager.post(Constants.ArticleService.RemoveImageUrl, data, OnMultipleMediaButtonActionCompleted, selectedurls);
}

function OnMultipleMediaButtonActionCompleted(success, data, selectedUrls) {
    var response = jQuery.parseJSON(data);
    if (response.success) {

        // remove all the images from ui, that has been removed from the gallery.
        var urls = selectedUrls.split(',');
        for (var i = 0; i < urls.length; i++) {
            //find the image with the source
            var stag = "'img[src$=\"" + urls[i] + "\"]'";
            var img = $(document).find(stag);
            if (img != null) {
                var div = img.parent();
                div.remove();
            }
        }


    }
    else {
        Notification.Show("#commonnotification", response.error, false /* autohide */);
    }

}

function OnArticleImagesSelected(images) {
    if (images != null && images.length > 0) {

        //photos is an array of string
        var photos = "";
        for (var i = 0; i < images.length; i++) {
            photos += "images=" + images[i];

            if (i != images.length - 1) {
                photos += "&";
            }
        }

        var urlkey = $("#theForm").find("#urlkey").val();

        // add these images to the gallery
        var data = "urlKey={0}&{1}".format(urlkey, photos);
        var posturl = Constants.ArticleService.AddImagesUrl;

        SavingNotification.Show();
        PostManager.post(posturl, data, OnArticleImagesAddedComplete, images);        
    }//if
}

// images has been added to article, update the UI
function OnArticleImagesAddedComplete(success, data, images) {

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
                    DisplayAddedImage(images[i]);
                }
            }
        }
    }
    catch (err) {
        message = err;
    }

    if (!osuccess) {
        SavingNotification.OnSavingComplete(osuccess, message);
    }

}
    
