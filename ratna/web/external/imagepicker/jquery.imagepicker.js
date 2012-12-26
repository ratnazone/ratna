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
/*
* ImagePicker v0.1 - jQuery Plugin
* http://jardalu.com/imagepicker/
* Copyright (c) Jardalu LLC (http://jardalu.com)
* Dual licensed under the MIT and GPL licenses
*/

(function ($) {

    var _cb = null; // callback to caller
    var searchUrl = "search.html";
    var searchSize = 12;
    var start = 1;
    var searchData = "query={0}&start={1}&size={2}";
    var maxPagesDisplay = 12;               // max number of pages that can be displayed in pager.

    var SearchingText = "Searching ...";
    var NoImagesFound = "No Images found.";
    var SearchError = "Error searching images";

    // html that will popup
    var popHtml = "<div class='messagepop pop'><div><input type='text' size='40' id='query' />" +
                  "<input type='submit' value='Search' id='image_search'/></div>" +
                  "<div id='popupbody'><span id='loadingSpan'>Loading...</span></div>" +
                  "<div id='mpager'></div>" +
                  "<div style='clear:both;text-align:right;'><input type='button' value='Ok' id='okButton' style='width:80px' />" +
                  "<input type='button' class='close' value='Cancel' id='cancelButton' style='width:80px'/></div>" +
                  "</div>";

    var imagePickerOn = null;
    var addedImages = new Array();

    var dynamicControls = {
        // values will get defined after the popup has been added
        // to the document.
        QueryInput: null,
        SearchButton: null,
        PopupDiv: null,
        CancelButton: null,
        OkButton: null,
        PopupBodyDiv: null,
        PagerDiv: null
    };

    var pagerRenderer = {

        minPagesDisplay: 5,
        initialSectionTotal: 2,
        lastSectionTotal: 2,

        render: function (current, total, pages) {

            // argument checking
            if (current < 1 || total < 1 || pages < 1 || (current > total) || pages < this.minPagesDisplay) {
                throw "argument error"
            }

            // calculate the endpoints
            var initialEnd = 0;
            var midStart = 0;
            var midEnd = 0;
            var lastStart = total - (this.lastSectionTotal - 1);

            // case : total less or equal to pages [all of them are set to total]
            if (total <= pages) {
                midStart = 1;
                midEnd = total;
                lastStart = total;
            }
            else {

                var delta = (pages - this.initialSectionTotal - this.lastSectionTotal);
                if (delta % 2 != 0) {
                    delta++;
                }

                delta = delta / 2;

                // set the middle section
                midStart = current - delta;
                midEnd = current + delta;

                var adjust = false;

                if (midStart <= this.initialSectionTotal) {
                    adjust = true;
                    gain = (midStart - 1);
                    midStart = 1;
                }

                if (midEnd >= (total - this.lastSectionTotal)) {
                    // adjust the loss in midEnd
                    midStart = midStart - (midEnd - total + this.lastSectionTotal);

                    midEnd = total;
                    lastStart = total;
                }

                if (adjust) {
                    //adjust for movement in midStart
                    midEnd = (pages - this.lastSectionTotal);
                }

                if (midStart > this.initialSectionTotal) {
                    initialEnd = this.initialSectionTotal;
                }


            } //else

            // display the endpoints
            var html = this.display(initialEnd, midStart, midEnd, lastStart, current, total);            
            dynamicControls.PagerDiv.html(html);
        },

        display: function (initialEnd, midStart, midEnd, lastStart, selected, total) {
            var i;

            var html = "<ul>";

            if (initialEnd > 0) {
                for (i = 1; i <= initialEnd; i++) {
                    html += this.getPageText(i, selected);
                }


                if (midStart > initialEnd + 2) {
                    html += this.getDividerText();
                }
                else if (midStart != (initialEnd + 1)) {
                    html += this.getPageText((initialEnd + 1), selected);
                }

            }

            for (i = midStart; i <= midEnd; i++) {
                html += this.getPageText(i, selected);
            }

            if (lastStart < total) {

                if (lastStart - midEnd > 2) {
                    html += this.getDividerText();
                }
                else {
                    html += this.getPageText((midEnd + 1), selected);
                }

                for (i = lastStart; i <= total; i++) {
                    html += this.getPageText(i, selected);
                }

            }

            html += "</ul>";

            return html;
        },

        getPageText: function (i, selected) {
            var text = null;

            if (i == selected) {
                text = "<li><span class='currentpage'>" + i + "</span></li>";
            }
            else {
                text = "<li><a class='pageclicked'>" + i + "</a></li>";
            }
            return text;
        },

        getDividerText: function () {
            return "<li class='divider'>...</li>";
        }

    };

    var methods = {

        deselect: function () {
            dynamicControls.PopupDiv.slideFadeToggle(function () {
                imagePickerOn.removeClass("selected");
            });

            // clear
            addedImages = new Array();
            dynamicControls.QueryInput.val("");
        },

        init: function () {
            this.searchImages();
        },

        searchImages: function () {

            dynamicControls.PopupBodyDiv.html(SearchingText);
            dynamicControls.PagerDiv.html("");

            var query = dynamicControls.QueryInput.val();

            //search for images on the remote call
            var data = searchData.format(query, start, searchSize);

            PostManager.post(searchUrl, data, this.onImagesSearchComplete);
        },

        onImagesSearchComplete: function (success, data) {
            var message = "";
            var urls = null;
            var qsuccess = false;
            var total = 0;

            try {
                var response = jQuery.parseJSON(data);

                if (!response.success) {
                    message = response.error;
                }
                else {
                    qsuccess = true;
                    urls = response.urls;
                    total = response.total;
                }
            }
            catch (err) {
                message = err;
            }

            // reset counters
            var selectedPage = Math.ceil((start - 1) / searchSize) + 1;
            start = 1;

            if (qsuccess) {
                methods.displaySearchedImages(urls, selectedPage, total);
            }
            else {
                // display error loading the images
                methods.displaySearchingImagesError();
            }
        },

        displaySearchingImagesError: function () {
            dynamicControls.PopupBodyDiv.html(SearchError);
        },

        displaySearchedImages: function (urls, selectedPage, total) {
            if (urls != null) {
                if (urls.length == 0) {
                    // no images found
                    dynamicControls.PopupBodyDiv.html(NoImagesFound);
                }
                else {
                    var imageHtml = "";

                    for (var i = 0; i < urls.length; i++) {
                        imageHtml += "<img class='selectable' style='width:50px;height:50px;' src='" + urls[i] + "'/>";
                    }

                    // set the images to display
                    dynamicControls.PopupBodyDiv.html(imageHtml);

                    // hover functionality
                    $('.selectable').mouseover(function () {
                        $(this).addClass('hover');
                    });

                    $('.selectable').mouseout(function () {
                        $(this).removeClass('hover');
                    });

                    // selectable can be clicked
                    $('.selectable').click(function () {
                        methods.AddSelectedImage(this);
                    });

                    //render pager
                    methods.RenderPager(selectedPage, total);
                }
            }
        },

        RenderPager: function (selectedPage, total) {

            //pagination
            if (total > searchSize) {

                //totalPages
                totalPages = Math.ceil(total / maxPagesDisplay);

                pagerRenderer.render(selectedPage, totalPages, maxPagesDisplay);

                // selected pages can be clicked
                $('.pageclicked').click(function () {
                    var page = $(this).text();
                    start = searchSize * (page-1) + 1;

                    //start search
                    methods.searchImages();
                });
            }

        },

        AddSelectedImage: function (image) {
            if (image != null) {
                $(image).addClass('imageSelected');

                addedImages[addedImages.length] = $(image).attr("src");
            }
        },

        okButtonClicked: function () {

            // callback
            if (typeof _cb == 'function') {
                _cb.call(this, addedImages);
            }

            this.deselect();
        }
    };

    $.fn.ImagePicker = function (options, callback) {

        _cb = callback;

        var settings = $.extend({
            'searchUrl': '/search.php',
            'searchSize': 12
        }, options);

        //set the searchurl location
        searchUrl = settings["searchUrl"];
        searchSize = settings["searchSize"];

        //add popHtml to the document
        $(popHtml).insertAfter(this);

        // initialize the dynamicVariables
        dynamicControls.QueryInput = $("#query");
        dynamicControls.SearchButton = $("#image_search");
        dynamicControls.PopupDiv = $(".pop");
        dynamicControls.CancelButton = $(".close");
        dynamicControls.OkButton = $("#okButton");
        dynamicControls.PopupBodyDiv = $("#popupbody");
        dynamicControls.PagerDiv = $("#mpager");

        imagePickerOn = this;

        this.click(function () {
            if ($(this).hasClass("selected")) {
                methods.deselect();
            } else {
                $(this).addClass("selected");

                dynamicControls.PopupDiv.slideFadeToggle(function () {
                    dynamicControls.QueryInput.focus();
                });

                methods.init();
            }
            return false;
        });

        //search click handler
        dynamicControls.SearchButton.click(function () {
            methods.searchImages();
            return false;
        });

        //cancel button
        dynamicControls.CancelButton.click(function () {
            methods.deselect();
            return false;
        });

        // ok button click
        dynamicControls.OkButton.click(function () {
            methods.okButtonClicked();
            return false;
        });
    };
})(jQuery);


$.fn.slideFadeToggle = function (easing, callback) {
    return this.animate({ opacity: 'toggle', height: 'toggle' }, "fast", easing, callback);
};
