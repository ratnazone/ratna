<%@ Page Title="" Language="C#" MasterPageFile="~/r-admin/template/admin.Master" AutoEventWireup="true" 
    CodeBehind="changepassword.aspx.cs" Inherits="Jardalu.Ratna.Web.Admin.pages.profile.changepassword" %>

<asp:Content ID="head" ContentPlaceHolderID="head" runat="server">
    <title><%= ResourceManager.GetLiteral("Admin.Profile.ChangePassword")%></title>

    <script type="text/javascript" src='<%=ResolveUrl("~/scripts/converter.js")%>'></script>
    <script type="text/javascript" src='<%=ResolveUrl("~/scripts/crypto/crypto-256.js")%>'></script>
    <script type="text/javascript" src='<%=ResolveUrl("~/scripts/encoders/base64.js")%>'></script>
    <script type="text/javascript" src='<%=ResolveUrl("~/scripts/encoders/UTF8.js")%>'></script>
    <script type="text/javascript" src='<%=ResolveUrl("~/external/validate/1.7/jquery.validate.js")%>'></script>

    <script type="text/javascript" src='<%=ResolveUrl("~/r-admin/scripts/profile/profile.changepassword.js")%>'></script>

</asp:Content>

<asp:Content ID="content" ContentPlaceHolderID="mainContentPlaceHolder" runat="server">

<div class="container_16 clearfix">
    <form id="changepasswordform" method="post" action="changepassword.aspx">
        <div class="login" id="mainDiv">
        <div>
            <h2>
                <asp:Literal runat="server" Text="<%$ Resource:Admin.Profile.ChangePassword%>" /></h2>        </div>
        <div id="errordiv" runat="server" visible="false">
            <p class="error" id="error" runat="server"></p>
        </div>
        <div id="successdiv" runat="server" visible="false">
            <p class="success" id="success" runat="server"><%= ResourceManager.GetLiteral("Admin.Profile.ChangePassword.Success")%></p>
        </div>
        <div>
            <label for="oldpassword">
                <asp:Literal id="oldpasswordLiteral" runat="server" Text="<%$ Resource:Admin.Profile.OldPassword%>" /></label>
                <input type="password" id="oldPassword" class="required" value="" />
        </div>
        <div>
            <label for="newpassword">
                <asp:Literal id="newpasswordLiteral" runat="server" Text="<%$ Resource:Admin.Profile.NewPassword%>" /></label>
                <input type="password" id="newPassword" class="required" value="" />
        </div>
        <div>
            <label for="retypenewpassword">
                <asp:Literal runat="server" Text="<%$ Resource:Admin.Profile.RetypeNewPassword%>" /></label>
                <input type="password" id="retypeNewPassword" class="required" equalTo="#newPassword" value="" />
        </div>
        <div>
            <input type="hidden" name="r" id="r" value="" runat="server"/>
            <input type="hidden" name="oldpassword" id="oldpasswordhash" value="" />
            <input type="hidden" name="newpassword" id="newpasswordhash" value="" />

            <input type="submit" value="<%= ResourceManager.GetLiteral("Admin.Common.Save")%>" id="savebutton" />
        </div>
    </div>
    </form>
</div>

</asp:Content>
