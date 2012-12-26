<%@ Page Title="" Language="C#" MasterPageFile="~/r-admin/template/admin.Master"
    AutoEventWireup="true" CodeBehind="editprofile.aspx.cs" Inherits="Jardalu.Ratna.Web.Admin.profile.editprofile" %>

<%@ Register TagPrefix="Common" TagName="SavingNotification" Src="~/Controls/Common/SavingNotification.ascx" %>
<asp:Content ID="head" ContentPlaceHolderID="head" runat="server">
    <title>
        <%= ResourceManager.GetLiteral("Admin.Profile.Edit.Title")%></title>
    <script type="text/javascript" src='<%=ResolveUrl("~/scripts/post.js")%>'></script>
    <script type="text/javascript" src='<%=ResolveUrl("~/scripts/savingnotification.js")%>'></script>
    <script type="text/javascript" src='<%=ResolveUrl("~/external/fuploader/fuploader.jquery.js")%>'></script>
    <script type="text/javascript" src='<%=ResolveUrl("~/external/form/form.jquery.js")%>'></script>
    <script type="text/javascript" src='<%=ResolveUrl("~/r-admin/scripts/profile/profile.edit.js")%>'></script>
    <script type="text/javascript">
        var CancelUrl = "<%= ResolveUrl(Constants.Urls.Profile.Url) %>";
    </script>

</asp:Content>
<asp:Content ID="mainContent" ContentPlaceHolderID="mainContentPlaceHolder" runat="server">
    <div id="content" class="container_16 clearfix">
        <div class="grid_16">
            <h1>
                <%= ResourceManager.GetLiteral("Admin.Profile.Edit.Header")%></h1>
        </div>
        <Common:SavingNotification id="savingNotification" runat="server" />
        <div class="grid_3">
            <img id="profileimage" class="userimage" src="~/images/gravatar.jpg" alt="profile" runat="server" /><br />
            <a href="javascript:void" id="photoselector">
                <%= ResourceManager.GetLiteral("Admin.Profile.Edit.ChangePhoto")%></a>
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
                <input type="text" class="grid_4" name="displayname" id="displayname" runat="server" /></div>
            <div class="grid_12">
                &nbsp;</div>
            <div class="grid_3">
                <%= ResourceManager.GetLiteral("Admin.Profile.FirstName")%></div>
            <div class="grid_9">
                <input type="text" class="grid_4" name="firstname" id="firstname" runat="server" /></div>
            <div class="grid_12">
                &nbsp;</div>
            <div class="grid_3">
                <%= ResourceManager.GetLiteral("Admin.Profile.LastName")%></div>
            <div class="grid_9">
                <input type="text" class="grid_4" name="lastname" id="lastname" runat="server" /></div>
            <div class="grid_12">
                &nbsp;</div>
        </div>
        <div class="grid_16" style="margin-top: 10px">
            <span>
                <%= ResourceManager.GetLiteral("Admin.Profile.Description")%></span>
        </div>
        <div class="grid_16" style="margin-top: 10px">
            <textarea rows="6" id="description" name="description" runat="server" class="grid_12" style="margin-left:0px"></textarea>
        </div>
        <div class="grid_16" style="margin-top: 10px">
            <input type="button" value="save" id="savebutton" />
            <input type="button" value="cancel" id="cancelbutton" />
        </div>
    </div>
    <div id="photouploaderformdiv">
        <form method="post" enctype="multipart/form-data" id="theForm" action="<%=ResolveUrl(Constants.Urls.Service.UploadUrl) %>">
        <input type="hidden" name="uploadtype" value="<%= Jardalu.Ratna.Web.Upload.UploadType.ProfilePhoto.ToString() %>" />
        </form>
    </div>
</asp:Content>
