<%@ Page Title="" Language="C#" MasterPageFile="~/r-admin/template/admin.Master" AutoEventWireup="true" CodeBehind="edit.aspx.cs" 
Inherits="Jardalu.Ratna.Web.Admin.pages.apps.edit" %>

<%@ Register TagPrefix="Ratna" TagName="PageAccess" Src="~/Controls/Common/PageAccess.ascx" %>
<%@ Register TagPrefix="Common" TagName="SavingNotification" Src="~/Controls/Common/SavingNotification.ascx" %>
<%@ Register TagPrefix="Common" TagName="Notification" Src="~/Controls/Common/Notification.ascx" %>
<%@ Register TagPrefix="Ratna" TagName="ClientJavaScript" Src="~/Controls/Common/ClientJavaScript.ascx" %>

<asp:Content ID="head" ContentPlaceHolderID="head" runat="server">
<Ratna:PageAccess ID="pageAccess" runat="server" IsRestricted="true" AllowGroups="Administrator" />
    <title>
        <%= ResourceManager.GetLiteral("Admin.Apps.Edit")%></title>

    <script type="text/javascript" src='<%=ResolveUrl("~/scripts/post.js")%>'></script>
    <script type="text/javascript" src='<%=ResolveUrl("~/scripts/notification.js")%>'></script>
    <script type="text/javascript" src='<%=ResolveUrl("~/scripts/savingnotification.js")%>'></script>

     <script type="text/javascript" src='<%=ResolveUrl("~/r-admin/scripts/apps/apps.edit.js")%>'></script>

     <Ratna:ClientJavaScript id="clientJavaScript" runat="server" />
</asp:Content>

<asp:Content ID="mainContent" ContentPlaceHolderID="mainContentPlaceHolder" runat="server">
    <div runat="server" id="content" class="container_16 clearfix">

        <!-- notification -->
        <Common:SavingNotification id="notification" runat="server" />
        <Common:Notification id="commonnotification" runat="server" />

        <div class="grid_3">
            <div style="text-align:center">
                <img src="" runat="server" id="applogo" style="width:120px;" />
                <br />
                <input type="button" id="activatebtn" runat="server" visible="false"/>
                <input type="button" id="deactivatebtn" runat="server" visible="false"/>
                <br />
                <input type="button" id="deletebtn"  value="<%= ResourceManager.GetLiteral("Admin.Common.Delete")%>"/>
            </div>
        </div>
        <div class="grid_13">
            <div>
                <h2><asp:Literal id="appname" runat="server" /></h2>
                <span id="description" runat="server"></span>
                <input type="hidden" runat="server" id="appid" value="" />
            </div>
            <div runat="server" id="fieldsDiv" style="margin-top:8px">
                <span style="display:block;font-weight:bold;font-size:13px;"><asp:Literal id="propertiesLiteral" runat="server" /></span>
                <span style="display:block;border-bottom:1px solid silver;" runat="server" id="propertieshelp"></span>

                <form id="savepropertiesform" name="savepropertiesform" method="get">
                    <div runat="server" id="fieldsInnerDiv" style="margin-top:10px;margin-bottom:10px;"></div>

                    <input id="saveproperties"  type="button" value="<%= ResourceManager.GetLiteral("Admin.Articles.Edit.Save")%>" 
                        title="<%= ResourceManager.GetLiteral("Admin.Articles.Edit.Save")%>"/>

                     <input id="cancelbtn"  type="button" value="<%= ResourceManager.GetLiteral("Admin.Articles.Edit.Cancel")%>" 
                        title="<%= ResourceManager.GetLiteral("Admin.Articles.Edit.Cancel")%>"/>
                </form>
            </div>
        </div>
    </div>

    <div runat="server" id="error" class="container_16 clearfix" visible="false">
        <div>
        <%= ResourceManager.GetLiteral("Admin.Apps.NoAppFound")%>
        </div>
    </div>

</asp:Content>
