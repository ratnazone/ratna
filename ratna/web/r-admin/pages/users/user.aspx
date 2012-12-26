<%@ Page Title="" Language="C#" MasterPageFile="~/r-admin/template/admin.Master"
    AutoEventWireup="true" CodeBehind="user.aspx.cs" Inherits="Jardalu.Ratna.Web.Admin.users.user" %>

<%@ Register TagPrefix="Ratna" TagName="PageAccess" Src="~/Controls/Common/PageAccess.ascx" %>
<%@ Register TagPrefix="Admin" TagName="UserMembership" Src="~/r-admin/controls/Users/UserMembership.ascx" %>

<asp:Content ID="head" ContentPlaceHolderID="head" runat="server">
    <Ratna:PageAccess id="pageAccess" runat="server" IsRestricted="true" AllowGroups="Administrator" />
    <title><%= ResourceManager.GetLiteral("Admin.Users.Title")%></title>
    <script type="text/javascript" src='<%=ResolveUrl("~/scripts/post.js")%>'></script>
</asp:Content>

<asp:Content ID="mainContent" ContentPlaceHolderID="mainContentPlaceHolder" runat="server">
    <div id="content" class="container_16 clearfix">

        <h1 id="headerH1" runat="server" class="grid_16"></h1>

        <div class="grid_3">
            <img id="userimage" class="userimage" src="~/images/gravatar.jpg" alt="user" runat="server" /><br />
        </div>
        <div class="grid_13">
            <div class="grid_3">
               <label><%= ResourceManager.GetLiteral("Admin.Profile.Alias")%></label></div>
            <div class="grid_9">
                <span id="aliasspan" runat="server"></span>
            </div>
            <div class="grid_12" style="height:4px;">
                &nbsp;</div>
            <div class="grid_3">
                 <label><%= ResourceManager.GetLiteral("Admin.Profile.Email")%> </label></div>
            <div class="grid_9">
                <span id="emailspan" runat="server"></span>
            </div>
            <div class="grid_12" style="height:4px;">
                &nbsp;</div>
            <div class="grid_3">
                 <label><%= ResourceManager.GetLiteral("Admin.Profile.DisplayName")%> </label></div>
            <div class="grid_9">
                <span id="displaynamespan" runat="server"></span>
            </div>
            <div class="grid_12" style="height:4px;">
                &nbsp;</div>
            <div class="grid_3">
                 <label><%= ResourceManager.GetLiteral("Admin.Profile.FirstName")%> </label></div>
            <div class="grid_9">
                <span id="firstnamespan" runat="server"></span>
            </div>
            <div class="grid_12" style="height:4px;">
                &nbsp;</div>
            <div class="grid_3">
                 <label><%= ResourceManager.GetLiteral("Admin.Profile.LastName")%> </label></div>
            <div class="grid_9">
                <span id="lastnamespan" runat="server"></span>
            </div>
            <div class="grid_12" style="height:4px;">
                &nbsp;</div>
                <div class="grid_3"> <label><%= ResourceManager.GetLiteral("Admin.Profile.LastSigned")%></label></div>
            <div class="grid_9">
                <span id="lastsigned" runat="server"></span>
            </div>
            <div class="grid_12" style="height:4px;">
                &nbsp;</div>
            <div class="grid_3"><label><%= ResourceManager.GetLiteral("Admin.Profile.CreatedOn")%></label></div>
            <div class="grid_9">
                <span id="createdon" runat="server"></span>
            </div>
            <div class="grid_12" style="height:4px;">
                &nbsp;</div>
        </div>
        <div class="grid_16" style="margin-top: 10px;margin-bottom: 10px">
            <span>
                <label><%= ResourceManager.GetLiteral("Admin.Profile.Description")%></label></span>
                <span id="descriptionspan" style="min-height:10px"
                    runat="server">&nbsp;</span>
        </div>
        
        <Admin:UserMembership id="usermembership" runat="server" />

    </div>

</asp:Content>
