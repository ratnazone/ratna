<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="photosummarycontrol.ascx.cs" Inherits="Jardalu.Ratna.Web.templates.system.photosummarycontrol" %>
<div style="display:inline-block;border:2px solid silver;padding:4px;margin-bottom:10px; margin-right:10px">
    <a class="fancyimage" href="<%= Photo.Url %>" title="<%= Photo.Name %>" ><img src='<%= Photo.Url %>' style="width:180px;height:120px;border:none;"/></a>
    <br /><span><%= Photo.Name %></span>
</div>