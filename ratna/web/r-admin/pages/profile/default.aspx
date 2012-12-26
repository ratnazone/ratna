<%@ Page Title="" Language="C#" MasterPageFile="~/r-admin/template/admin.Master"
    AutoEventWireup="true" CodeBehind="default.aspx.cs" Inherits="Jardalu.Ratna.Web.Admin.Profile._default" %>

<asp:Content ID="head" ContentPlaceHolderID="head" runat="server">
    <title>
        <%= ResourceManager.GetLiteral("Admin.Profile.PageTitle")%></title>
</asp:Content>
<asp:Content ID="mainContent" ContentPlaceHolderID="mainContentPlaceHolder" runat="server">
    <div id="content" class="container_16 clearfix">
        <div class="grid_16">
            <h1>
                <%= ResourceManager.GetLiteral("Admin.Profile.Header")%></h1>
                <div class="utils">
							<a href="editprofile.aspx"><%= ResourceManager.GetLiteral("Admin.Profile.Edit")%></a>
						</div>
        </div>
        <div class="grid_3">
            <img id="profileimage" class="userimage" src="~/images/gravatar.jpg" alt="profile" runat="server" /><br />
        </div>
        <div class="grid_13">
            <div class="grid_3">
                <%= ResourceManager.GetLiteral("Admin.Profile.Alias")%></div>
            <div class="grid_9">
                <span id="aliasspan" runat="server"></span>
            </div>
            <div class="grid_12">
                &nbsp;</div>
            <div class="grid_3">
                <%= ResourceManager.GetLiteral("Admin.Profile.Email")%></div>
            <div class="grid_9">
                <span id="emailspan" runat="server"></span>
            </div>
            <div class="grid_12">
                &nbsp;</div>
            <div class="grid_3">
                <%= ResourceManager.GetLiteral("Admin.Profile.DisplayName")%></div>
            <div class="grid_9">
                <span id="displaynamespan" runat="server"></span>
            </div>
            <div class="grid_12">
                &nbsp;</div>
            <div class="grid_3">
                <%= ResourceManager.GetLiteral("Admin.Profile.FirstName")%></div>
            <div class="grid_9">
                <span id="firstnamespan" runat="server"></span>
            </div>
            <div class="grid_12">
                &nbsp;</div>
            <div class="grid_3">
                <%= ResourceManager.GetLiteral("Admin.Profile.LastName")%></div>
            <div class="grid_9">
                <span id="lastnamespan" runat="server"></span>
            </div>
            <div class="grid_12">
                &nbsp;</div>
        </div>
        <div class="grid_16" style="margin-top: 10px">
            <span style="display:block">
                <%= ResourceManager.GetLiteral("Admin.Profile.Description")%></span> 
            <span id="descriptionspan"
                    runat="server"></span>
        </div>
    </div>
</asp:Content>
