<%@ Page Title="" Language="C#" MasterPageFile="~/r-admin/template/admin.Master" AutoEventWireup="true" CodeBehind="smtp.aspx.cs" 
    Inherits="Jardalu.Ratna.Web.Admin.pages.settings.smtp" %>

<%@ Register TagPrefix="Ratna" TagName="PageAccess" Src="~/Controls/Common/PageAccess.ascx" %>
<%@ Register TagPrefix="Admin" TagName="Menu" Src="~/r-admin/controls/Menu.ascx" %>
<%@ Register TagPrefix="Common" TagName="Notification" Src="~/Controls/Common/Notification.ascx" %>
<%@ Register TagPrefix="Ratna" TagName="ClientJavaScript" Src="~/Controls/Common/ClientJavaScript.ascx" %>

<asp:Content ID="head" ContentPlaceHolderID="head" runat="server">
    <Ratna:PageAccess id="pageAccess" runat="server" IsRestricted="true" AllowGroups="Administrator" />

    <Ratna:ClientJavaScript id="clientJavaScript" runat="server" />

    <script type="text/javascript" src='<%=ResolveUrl("~/scripts/post.js")%>'></script>
    <script type="text/javascript" src='<%=ResolveUrl("~/scripts/notification.js")%>'></script>
    <script type="text/javascript" src='<%=ResolveUrl("~/r-admin/scripts/settings/settings.notification.js")%>'></script>
</asp:Content>

<asp:Content ID="mainContent" ContentPlaceHolderID="mainContentPlaceHolder" runat="server">

<div id="content" class="container_16 clearfix" runat="server"> 
        <div class="grid_16">
            <h1><asp:Literal id="headerLiteral" runat="server" Text="<%$ Resource:Admin.Settings.Smtp.Title %>" /></h1>
        </div>

        <div class="grid_3 alpha">
            <Admin:Menu id="menu" runat="server" />
        </div>

        <div class="grid_13 omega">

            <Common:Notification id="cnotification" runat="server" />

            <div class="grid_13 bottommargin">
                <p>
                    <label for="smtpaddress">
                        <%=  ResourceManager.GetLiteral("Admin.Settings.Smtp.Address")%>
                        <small>
                            <%=  ResourceManager.GetLiteral("Admin.Settings.Smtp.Address.Help")%></small>
                    </label>
                    <input type="text" name="smtpaddress" id="smtpaddress" runat="server" class="grid_7 nomargin" />
                </p>
            </div>

            <div class="grid_13 bottommargin">
                <p>
                    <label for="fromemail">
                        <%=  ResourceManager.GetLiteral("Admin.Settings.Smtp.From")%>
                        <small>
                            <%=  ResourceManager.GetLiteral("Admin.Settings.Smtp.From.Help")%></small>
                    </label>
                    <input type="text" name="fromemail" id="fromemail" runat="server" class="grid_7 nomargin" />
                </p>
            </div>

            <div class="grid_13 bottommargin">
                <p>
                    <label for="username">
                        <%=  ResourceManager.GetLiteral("Admin.Settings.Smtp.UserName")%>
                        <small>
                            <%=  ResourceManager.GetLiteral("Admin.Settings.Smtp.UserName.Help")%></small>
                    </label>
                    <input type="text" name="smtpusername" id="smtpusername" runat="server"  class="grid_7 nomargin"/>
                </p>
            </div>

            <div class="grid_13 bottommargin">
                <p>
                    <label for="password">
                        <%=  ResourceManager.GetLiteral("Admin.Settings.Smtp.Password")%>
                        <small>
                            <%=  ResourceManager.GetLiteral("Admin.Settings.Smtp.Password.Help")%></small>
                    </label>
                    <input type="password" name="smtppassword" id="smtppassword" runat="server"  class="grid_7 nomargin"/>
                </p>
            </div>

            <div class="grid_13">
                <input type="button" id="smtpSettingSaveButton" runat="server" value="<%$Resource:Admin.Common.Save %>" />
                <input type="button" id="smtpSettingCancelButton" runat="server" value="<%$Resource:Admin.Common.Cancel %>" />
            </div>

        </div>

</div>

</asp:Content>
