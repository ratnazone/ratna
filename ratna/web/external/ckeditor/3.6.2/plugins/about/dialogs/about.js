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

CKEDITOR.dialog.add('about',function(a){var b=a.lang.about;return{title:CKEDITOR.env.ie?b.dlgTitle:b.title,minWidth:390,minHeight:230,contents:[{id:'tab1',label:'',title:'',expand:true,padding:0,elements:[{type:'html',html:'<style type="text/css">.cke_about_container{color:#000 !important;padding:10px 10px 0;margin-top:5px}.cke_about_container p{margin: 0 0 10px;}.cke_about_container .cke_about_logo{height:81px;background-color:#fff;background-image:url('+CKEDITOR.plugins.get('about').path+'dialogs/logo_ckeditor.png);'+'background-position:center; '+'background-repeat:no-repeat;'+'margin-bottom:10px;'+'}'+'.cke_about_container a'+'{'+'cursor:pointer !important;'+'color:blue !important;'+'text-decoration:underline !important;'+'}'+'</style>'+'<div class="cke_about_container">'+'<div class="cke_about_logo"></div>'+'<p>'+'CKEditor '+CKEDITOR.version+' (revision '+CKEDITOR.revision+')<br>'+'<a href="http://ckeditor.com/">http://ckeditor.com</a>'+'</p>'+'<p>'+b.help.replace('$1','<a href="http://docs.cksource.com/CKEditor_3.x/Users_Guide/Quick_Reference">'+b.userGuide+'</a>')+'</p>'+'<p>'+b.moreInfo+'<br>'+'<a href="http://ckeditor.com/license">http://ckeditor.com/license</a>'+'</p>'+'<p>'+b.copy.replace('$1','<a href="http://cksource.com/">CKSource</a> - Frederico Knabben')+'</p>'+'</div>'}]}],buttons:[CKEDITOR.dialog.cancelButton]};});
