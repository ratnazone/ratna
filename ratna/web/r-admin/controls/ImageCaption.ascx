<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ImageCaption.ascx.cs" Inherits="Jardalu.Ratna.Web.Admin.controls.ImageCaption" %>

<div class="caption-image" style="width:140px;text-align:center;display:inline-block"> 
    <img 
        src="<%= ImageUrl %>" 
        width="120" 
        height="60" 
        /> 
    <br />
    <%= Caption %>
</div>
