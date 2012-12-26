<%@ Page Title="" Language="C#" MasterPageFile="~/r-admin/template/admin.Master"
    AutoEventWireup="true" CodeBehind="editform.aspx.cs" Inherits="Jardalu.Ratna.Web.Admin.pages.forms.editform" %>

<%@ Register TagPrefix="Ratna" TagName="PageAccess" Src="~/Controls/Common/PageAccess.ascx" %>
<%@ Register TagPrefix="Common" TagName="SavingNotification" Src="~/Controls/Common/SavingNotification.ascx" %>
<%@ Register TagPrefix="Common" TagName="Notification" Src="~/Controls/Common/Notification.ascx" %>
<%@ Register TagPrefix="Admin" TagName="FormFieldRowControl" Src="~/r-admin/controls/Forms/FormFieldRowControl.ascx" %>
<%@ Register TagPrefix="Ratna" TagName="ClientJavaScript" Src="~/Controls/Common/ClientJavaScript.ascx" %>
<%@ Register TagPrefix="Ratna" TagName="ConfirmDialog" Src="~/Controls/Common/ConfirmDialog.ascx" %>

<asp:Content ID="head" ContentPlaceHolderID="head" runat="server">
    <Ratna:PageAccess ID="pageAccess" runat="server" IsRestricted="true" AllowGroups="Administrator" />
    <script type="text/javascript" src='<%=ResolveUrl("~/external/validate/1.7/jquery.validate.js")%>'></script>
    <script type="text/javascript" src='<%=ResolveUrl("~/r-admin/scripts/forms/forms.editform.js")%>'></script>    
    <script type="text/javascript" src='<%=ResolveUrl("~/scripts/post.js")%>'></script>
    <script type="text/javascript" src='<%=ResolveUrl("~/scripts/savingnotification.js")%>'></script>
    <script type="text/javascript" src='<%=ResolveUrl("~/scripts/notification.js")%>'></script>

    <Ratna:ClientJavaScript id="clientJavaScript" runat="server" />

</asp:Content>

<asp:Content ID="maincontent" ContentPlaceHolderID="mainContentPlaceHolder" runat="server">
    <Ratna:ConfirmDialog id="confirmDialog" runat="server" />
    <div id="content" class="container_16 clearfix">
        
        <form id="editform" method="get" action="">
        <div class="grid_16">
            <h1>
                <asp:Literal id="headerLiteral" runat="server" /></h1>
        </div>

        <Common:SavingNotification id="savingNotification" runat="server" />
        <Common:Notification id="commonnotification" runat="server" Class="grid_15"/>

        <div class="grid_3">
            <%= ResourceManager.GetLiteral("Admin.Forms.Name")%></div>
        <div class="grid_4">
            <input type="text" class="required" name="name" id="name" runat="server" />
            <input type="hidden" name="uid" id="uid" runat="server" />
        </div>
        <div class="grid_16">
            &nbsp;</div>
        <div class="grid_3">
            <%= ResourceManager.GetLiteral("Admin.Forms.DisplayName")%></div>
        <div class="grid_4">
            <input type="text" class="required" name="displayname" id="displayname" runat="server" />
        </div>
        <div class="grid_16">
            &nbsp;</div>
        <div class="grid_16" style="text-align:right">
            <p>
                <input type="submit" value="<%= ResourceManager.GetLiteral("Admin.Common.Save")%>"
                    id="savebutton" />
                <input type="reset" value="<%= ResourceManager.GetLiteral("Admin.Common.Cancel")%>"
                    id="cancelbutton" />
            </p>
        </div>
        <div class="grid_16">
            <h2>
                <%= ResourceManager.GetLiteral("Admin.Forms.Fields")%></h2>
        </div>
        <table>
            <thead>
                <tr>
                    <th>
                        <%= ResourceManager.GetLiteral("Admin.Forms.FieldName")%>
                    </th>
                    <th>
                        <%= ResourceManager.GetLiteral("Admin.Forms.FieldType")%>
                    </th>
                    <th>
                        <%= ResourceManager.GetLiteral("Admin.Forms.FieldRequired")%>
                    </th>
                    <th>
                        &nbsp;
                    </th>
                </tr>
            </thead>
            <tbody>
                <Admin:FormFieldRowControl ID="formFieldRowControl" runat="server" />
                <tr id="addnewfieldtr">
                    <td>
                        <input type="text" name="newfieldname" id="newfieldname" size="32" />
                    </td>
                    <td>
                        <select id="newfieldselect" name="newfieldselect" runat="server">
                        </select>
                    </td>
                    <td>
                        <input type="checkbox" id="newfieldrequired" name="newfieldrequired" />
                    </td>
                    <td>
                        <img alt="<%$ Resource:Admin.Forms.Field.New%>" title="<%$ Resource:Admin.Forms.Field.New%>"
                            class="imageButton16" id="addNewField" src="/images/save.png" runat="server" />
                    </td>
                </tr>
            </tbody>
        </table>
        </form>
    </div>
</asp:Content>
