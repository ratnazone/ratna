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
    original code : http://www.daimi.au.dk/~u061768/file-input.html 

    This file has been modified to suit the needs of Ratna.
*/

jQuery.fn.choose = function (f) {
    $(this).bind('choose', f);
};


jQuery.fn.file = function () {
    return this.each(function () {
        var btn = $(this);
        var pos = btn.offset();

        function update() {
            pos = btn.offset();
            file.css({
                'top': pos.top,
                'left': pos.left,
                'width': btn.width(),
                'height': btn.height()
            });
        }

        btn.mouseover(update);

        var hidden = $('<div></div>').css({
            'display': 'none'
        }).appendTo('body');

        var file = $('<div><form></form></div>').appendTo('body').css({
            'position': 'absolute',
            'overflow': 'hidden',
            '-moz-opacity': '0',
            'filter': 'alpha(opacity: 0)',
            'opacity': '0',
            'z-index': '2'
        });

        var form = file.find('form');
        var input = form.find('input');

        function reset() {
            var input = $('<input type="file" multiple name="f">').appendTo(form);
            input.change(function (e) {
                input.unbind();
                input.detach();
                btn.trigger('choose', [input]);
                reset();
            });
        };

        reset();

        function placer(e) {
            form.css('margin-left', e.pageX - pos.left - offset.width);
            form.css('margin-top', e.pageY - pos.top - offset.height + 3);
        }

        function redirect(name) {
            file[name](function (e) {
                btn.trigger(name);
            });
        }

        file.mousemove(placer);
        btn.mousemove(placer);

        redirect('mouseover');
        redirect('mouseout');
        redirect('mousedown');
        redirect('mouseup');

        var offset = {
            width: file.width() - 25,
            height: file.height() / 2
        };

        update();
    });
};
