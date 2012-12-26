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

// Compressed version of core/ckeditor_base.js. See original for instructions.
/*jsl:ignore*/
if(!window.CKEDITOR)window.CKEDITOR=(function(){var a={timestamp:'',version:'3.6.2',revision:'7275',_:{},status:'unloaded',basePath:(function(){var d=window.CKEDITOR_BASEPATH||'';if(!d){var e=document.getElementsByTagName('script');for(var f=0;f<e.length;f++){var g=e[f].src.match(/(^|.*[\\\/])ckeditor(?:_basic)?(?:_source)?.js(?:\?.*)?$/i);if(g){d=g[1];break;}}}if(d.indexOf(':/')==-1)if(d.indexOf('/')===0)d=location.href.match(/^.*?:\/\/[^\/]*/)[0]+d;else d=location.href.match(/^[^\?]*\/(?:)/)[0]+d;return d;})(),getUrl:function(d){if(d.indexOf(':/')==-1&&d.indexOf('/')!==0)d=this.basePath+d;if(this.timestamp&&d.charAt(d.length-1)!='/')d+=(d.indexOf('?')>=0?'&':'?')+('t=')+this.timestamp;return d;}},b=window.CKEDITOR_GETURL;if(b){var c=a.getUrl;a.getUrl=function(d){return b.call(a,d)||c.call(a,d);};}return a;})();
/*jsl:end*/

// Uncomment the following line to have a new timestamp generated for each
// request, having clear cache load of the editor code.
// CKEDITOR.timestamp = ( new Date() ).valueOf();

// Set the script name to be loaded by the loader.
CKEDITOR._autoLoad = 'core/ckeditor_basic';

// Include the loader script.
document.write(
	'<script type="text/javascript" src="' + CKEDITOR.getUrl( '_source/core/loader.js' ) + '"></script>' );
