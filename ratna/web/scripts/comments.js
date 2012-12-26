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
var CommentForm = {

    submit: function (formname) {
        CommentForm.submit(formname, CommentForm.OnFormSubmitted);
    },

    submit: function (formname, callback) {

        var form = $('#' + formname);

        var key = form.find("#key").val();
        var name = form.find("#name").val();
        var email = form.find("#email").val();
        var url = form.find("#url").val();
        var body = form.find("#message").val();
        var permalink = form.find("#permalink").val();
        var threadrenderer = form.find("#threadrenderer").val();

        var data = "key={0}&name={1}&email={2}&url={3}&body={4}&permalink={5}&threadrenderer={6}".formatEscape(key, name, email, url, body, permalink, threadrenderer);

        //PostManager.post(Constants.CommentsPublicService.AddCommentUrl, data, CommentForm.OnFormSubmitted, formname);
        PostManager.post(Constants.CommentsPublicService.AddCommentUrl, data, callback, formname);
    },

    OnFormSubmitted: function (success, data, formname) {
        var response = jQuery.parseJSON(data);
        if (success && response.success) {
            var html = response.html;
            if (html != null) {
                $(html).insertBefore("#comments-bottom-marker");
            }
        }
    }

}
