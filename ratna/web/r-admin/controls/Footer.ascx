<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Footer.ascx.cs" Inherits="Jardalu.Ratna.Web.Admin.controls.Footer" %>
<div id="foot">
    <div class="container_16 clearfix">
        <div class="grid_13">
            <a href="~/r-admin/pages/profile/changepassword.aspx" runat="server"><%= ResourceManager.GetLiteral("Admin.Profile.ChangePassword")%></a>
            <a href="~/r-admin/logout.aspx" runat="server"><%= ResourceManager.GetLiteral("Admin.Footer.SignOut")%></a>
        </div>
        <div class="grid_3">
            <span style="float:right">Version : <%= Version %></span>
        </div>
    </div>
</div>
