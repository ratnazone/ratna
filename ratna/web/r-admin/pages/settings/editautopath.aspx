<%@ Page Language="C#" MasterPageFile="~/r-admin/template/admin.Master" AutoEventWireup="true" CodeBehind="editautopath.aspx.cs" 
Inherits="Jardalu.Ratna.Web.Admin.pages.settings.editautopath" %>

<%@ Register TagPrefix="Ratna" TagName="PageAccess" Src="~/Controls/Common/PageAccess.ascx" %>
<%@ Register TagPrefix="Admin" TagName="Menu" Src="~/r-admin/controls/Menu.ascx" %>
<%@ Register TagPrefix="Common" TagName="Notification" Src="~/Controls/Common/Notification.ascx" %>
<%@ Register TagPrefix="Common" TagName="SavingNotification" Src="~/Controls/Common/SavingNotification.ascx" %>
<%@ Register TagPrefix="Ratna" TagName="ClientJavaScript" Src="~/Controls/Common/ClientJavaScript.ascx" %>

<asp:Content ID="head" ContentPlaceHolderID="head" runat="server">
    <Ratna:PageAccess id="pageAccess" runat="server" IsRestricted="true" AllowGroups="Administrator" />

    <Ratna:ClientJavaScript id="clientJavaScript" runat="server" />

    <script type="text/javascript" src='<%=ResolveUrl("~/scripts/post.js")%>'></script>
    <script type="text/javascript" src='<%=ResolveUrl("~/scripts/savingnotification.js")%>'></script>
    <script type="text/javascript" src='<%=ResolveUrl("~/scripts/notification.js")%>'></script>
    <script type="text/javascript" src='<%=ResolveUrl("~/external/validate/1.7/jquery.validate.js")%>'></script>
    <script type="text/javascript" src='<%=ResolveUrl("~/r-admin/scripts/settings/autopaths.edit.js")%>'></script>
</asp:Content>

<asp:Content ID="mainContent" ContentPlaceHolderID="mainContentPlaceHolder" runat="server">
    <div id="content" class="container_16 clearfix" runat="server"> 
        <div class="grid_16">
            <h1><asp:Literal id="headerLiteral" runat="server" /></h1>
        </div>

        <div class="grid_3 alpha">
            <Admin:Menu id="menu" runat="server" />
        </div>

        <div class="grid_13 omega">
            <Common:SavingNotification id="savingNotification" runat="server" ClassName="grid_13"/>
            <Common:Notification id="cnotification" runat="server" />

            <form id="autopath_form" method="get" action="" runat="server">
            <div class="grid_13 bottommargin">
                <p>
                    <label for="pathtitle">
                        <%=  ResourceManager.GetLiteral("Admin.Common.Title")%>
                        <small>
                            <%=  ResourceManager.GetLiteral("Admin.Settings.AutoPaths.Title.Help")%></small>
                    </label>
                    <input type="text" name="pathtitle" id="pathtitle" runat="server" class="grid_7 nomargin required" />
                </p>
            </div>

            <div class="grid_13 bottommargin">
                <p>
                    <label for="pathurl">
                        <%=  ResourceManager.GetLiteral("Admin.Settings.AutoPaths.PathUrl")%>
                        <small>
                            <%=  ResourceManager.GetLiteral("Admin.Settings.AutoPaths.PathUrl.Help")%></small>
                    </label>
                    <input type="text" name="pathurl" id="pathurl" runat="server" class="grid_7 nomargin required relurl" />
                </p>
            </div>

            <div class="grid_13">
                <p>
                    <label for="pathTypesSelect"><%= ResourceManager.GetLiteral("Admin.Settings.AutoPaths.PathType")%></label>
                    <select id="pathTypesSelect" runat="server" />
                </p>
            </div>

            <div class="grid_13 bottommargin">
                <p>
                    <label for="navigation">
                        <%=  ResourceManager.GetLiteral("Admin.Settings.AutoPaths.Navigation")%>
                        <small>
                            <%=  ResourceManager.GetLiteral("Admin.Settings.AutoPaths.Navigation.Help")%></small>
                    </label>
                    <input type="text" name="pathNavigation" id="pathNavigation" runat="server" class="grid_7 nomargin" />
                </p>
            </div>

            <div class="grid_13 bottommargin">
                <p>
                    <label for="pagesize"><%= ResourceManager.GetLiteral("Admin.Settings.AutoPaths.PageSize")%>
                     <small>
                            <%=  ResourceManager.GetLiteral("Admin.Settings.AutoPaths.PageSize.Help")%></small>
                    </label>
                    <input type="text" name="pagesize" id="pagesize" runat="server" class="grid_3 nomargin digits" />
                </p>
            </div>

            <div class="grid_13" style="margin-top:10px;">
                <input type="submit" id="savebutton" runat="server" value="<%$Resource:Admin.Common.Save %>" />
                <input type="button" id="cancelbutton" runat="server" value="<%$Resource:Admin.Common.Cancel %>" />
            </div>

           </form>
        </div>
    </div>
</asp:Content>
