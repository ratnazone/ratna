<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AppListItem.ascx.cs" Inherits="Jardalu.Ratna.Web.Admin.controls.Apps.AppListItem" %>


<div class="caption-image" style="width:140px;text-align:center;display:inline-block;margin:10px;"> 
    <img 
        src="<%= IconUrl %>" 
        width="120" 
        height="60" 
        style="margin-top:5px"
        /> 
    <br />
    <span style="font-size:12px;font-weight:bold"><%= Name %></span>
    <br />
    <span style="float:left">
        <%= Description %>
        <a href="" runat="server" id="editLink"><%= ResourceManager.GetLiteral("Admin.Apps.Edit")%></a>
    </span>
</div>
