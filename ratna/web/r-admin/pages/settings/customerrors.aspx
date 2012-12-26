<%@ Page Title="" Language="C#" MasterPageFile="~/r-admin/template/admin.Master" AutoEventWireup="true" 
    CodeBehind="customerrors.aspx.cs" Inherits="Jardalu.Ratna.Web.Admin.pages.settings.customerrors" %>

<%@ Register TagPrefix="Ratna" TagName="PageAccess" Src="~/Controls/Common/PageAccess.ascx" %>
<%@ Register TagPrefix="Admin" TagName="Menu" Src="~/r-admin/controls/Menu.ascx" %>
<%@ Register TagPrefix="Common" TagName="Notification" Src="~/Controls/Common/Notification.ascx" %>
<%@ Register TagPrefix="Ratna" TagName="ClientJavaScript" Src="~/Controls/Common/ClientJavaScript.ascx" %>

<asp:Content ID="head" ContentPlaceHolderID="head" runat="server">
    <Ratna:PageAccess id="pageAccess" runat="server" IsRestricted="true" AllowGroups="Administrator" />

    <Ratna:ClientJavaScript id="clientJavaScript" runat="server" />

    <script type="text/javascript" src='<%=ResolveUrl("~/scripts/post.js")%>'></script>
    <script type="text/javascript" src='<%=ResolveUrl("~/scripts/notification.js")%>'></script>
    <script type="text/javascript" src='<%=ResolveUrl("~/r-admin/scripts/settings/settings.customerrors.js")%>'></script>
</asp:Content>

<asp:Content ID="mainContent" ContentPlaceHolderID="mainContentPlaceHolder" runat="server">


<div id="content" class="container_16 clearfix" runat="server"> 
    <div class="grid_16">
        <h1><asp:Literal id="headerLiteral" runat="server" Text="<%$ Resource:Admin.Settings.CustomErrors.Title %>" /></h1>
    </div>

    <div class="grid_3 alpha">
        <Admin:Menu id="menu" runat="server" />
    </div>

    <div class="grid_13 omega">
        <Common:Notification id="cnotification" runat="server" />

        <div class="grid_13 bottommargin">
            <p>
                <label for="error404">
                    <%=  ResourceManager.GetLiteral("Admin.Settings.CustomErrors.Error404")%>
                    <small>
                        <%=  ResourceManager.GetLiteral("Admin.Settings.CustomErrors.Error404.Help")%></small>
                </label>
                <input type="text" name="error404" id="error404" runat="server" class="grid_7 nomargin" />
            </p>
        </div>

         <div class="grid_13 bottommargin">
            <p>
                <label for="error500">
                    <%=  ResourceManager.GetLiteral("Admin.Settings.CustomErrors.Error500")%>
                    <small>
                        <%=  ResourceManager.GetLiteral("Admin.Settings.CustomErrors.Error500.Help")%></small>
                </label>
                <input type="text" name="error500" id="error500" runat="server" class="grid_7 nomargin" />
            </p>
        </div>

         <div class="grid_13 bottommargin">
            <p>
                <label for="errorothers">
                    <%=  ResourceManager.GetLiteral("Admin.Settings.CustomErrors.ErrorOthers")%>
                    <small>
                        <%=  ResourceManager.GetLiteral("Admin.Settings.CustomErrors.ErrorOthers.Help")%></small>
                </label>
                <input type="text" name="errorothers" id="errorothers" runat="server" class="grid_7 nomargin" />
            </p>
        </div>
        
        <div class="grid_13">
            <input type="button" id="customErrorsSaveButton" runat="server" value="<%$Resource:Admin.Common.Save %>" />
            <input type="button" id="customErrorsCancelButton" runat="server" value="<%$Resource:Admin.Common.Cancel %>" />
        </div>
    </div>

</div>

</asp:Content>
