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

CKEDITOR.skins.add('office2003',(function(){return{editor:{css:['editor.css']},dialog:{css:['dialog.css']},separator:{canGroup:false},templates:{css:['templates.css']},margins:[0,14,18,14]};})());(function(){CKEDITOR.dialog?a():CKEDITOR.on('dialogPluginReady',a);function a(){CKEDITOR.dialog.on('resize',function(b){var c=b.data,d=c.width,e=c.height,f=c.dialog,g=f.parts.contents;if(c.skin!='office2003')return;g.setStyles({width:d+'px',height:e+'px'});if(!CKEDITOR.env.ie||CKEDITOR.env.ie9Compat)return;var h=function(){var i=f.parts.dialog.getChild([0,0,0]),j=i.getChild(0),k=j.getSize('width');e+=j.getChild(0).getSize('height')+1;var l=i.getChild(2);l.setSize('width',k);l=i.getChild(7);l.setSize('width',k-28);l=i.getChild(4);l.setSize('height',e);l=i.getChild(5);l.setSize('height',e);};setTimeout(h,100);if(b.editor.lang.dir=='rtl')setTimeout(h,1000);});};})();
