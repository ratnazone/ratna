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
Copyright (c) 2003-2011, CKSource - Frederico Knabben. All rights reserved.
For licensing, see LICENSE.html or http://ckeditor.com/license
*/

(function(){CKEDITOR.plugins.add('ajax',{requires:['xml']});CKEDITOR.ajax=(function(){var a=function(){if(!CKEDITOR.env.ie||location.protocol!='file:')try{return new XMLHttpRequest();}catch(f){}try{return new ActiveXObject('Msxml2.XMLHTTP');}catch(g){}try{return new ActiveXObject('Microsoft.XMLHTTP');}catch(h){}return null;},b=function(f){return f.readyState==4&&(f.status>=200&&f.status<300||f.status==304||f.status===0||f.status==1223);},c=function(f){if(b(f))return f.responseText;return null;},d=function(f){if(b(f)){var g=f.responseXML;return new CKEDITOR.xml(g&&g.firstChild?g:f.responseText);}return null;},e=function(f,g,h){var i=!!g,j=a();if(!j)return null;j.open('GET',f,i);if(i)j.onreadystatechange=function(){if(j.readyState==4){g(h(j));j=null;}};j.send(null);return i?'':h(j);};return{load:function(f,g){return e(f,g,c);},loadXml:function(f,g){return e(f,g,d);}};})();})();
