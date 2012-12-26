<%@ Page Title="" Language="C#" MasterPageFile="~/r-admin/template/admin.Master"
    AutoEventWireup="true" CodeBehind="editentry.aspx.cs" Inherits="Jardalu.Ratna.Web.Admin.pages.forms.editentry" %>

<%@ Register TagPrefix="Ratna" TagName="PageAccess" Src="~/Controls/Common/PageAccess.ascx" %>
<%@ Register TagPrefix="Common" TagName="SavingNotification" Src="~/Controls/Common/SavingNotification.ascx" %>
<%@ Register TagPrefix="Admin" TagName="ActionPanel" Src="~/r-admin/controls/ActionPanel.ascx" %>

<asp:Content ID="head" ContentPlaceHolderID="head" runat="server">
    <Ratna:PageAccess ID="pageAccess" runat="server" IsRestricted="true" AllowGroups="Administrator" />
    <script type="text/javascript" src='<%=ResolveUrl("~/external/validate/1.7/jquery.validate.js")%>'></script>
    <script type="text/javascript" src='<%=ResolveUrl("~/r-admin/scripts/forms/forms.editentry.js")%>'></script>
    <script type="text/javascript" src='<%=ResolveUrl("~/scripts/post.js")%>'></script>
    <script type="text/javascript" src='<%=ResolveUrl("~/scripts/savingnotification.js")%>'></script>
    <script type="text/javascript" src='<%=ResolveUrl("~/scripts/notification.js")%>'></script>
    <script type="text/javascript" src='<%=ResolveUrl("~/external/datepicker/date.js")%>'></script>
    <script type="text/javascript" src='<%=ResolveUrl("~/external/datepicker/jquery.datePicker.js")%>'></script>
    <link rel="stylesheet" href='<%=ResolveUrl("~/external/datepicker/datePicker.css")%>'
        type="text/css" />
    <link rel="stylesheet" href='<%=ResolveUrl("~/external/datepicker/date.css")%>' type="text/css" />
    <script language="javascript" type="text/javascript">
        Date.format = 'mm/dd/yyyy';
        $(function () {
            $('.date-pick').datePicker({ startDate: '01/01/1972' });
        });
    </script>
</asp:Content>

<asp:Content ID="mainContent" ContentPlaceHolderID="mainContentPlaceHolder" runat="server">
    <div id="content" class="container_16 clearfix">
        <Common:SavingNotification ID="savingNotification" runat="server"/>
        <div class="grid_3">
            <admin:actionpanel runat="server" id="actionPanel" />
        </div>
        <div class="grid_13">
            <h2><asp:Literal runat="server" ID="headerLiteral" /></h2>
            <form id="editentryform" method="get" action="javascript:return false;">
            <input type="hidden" runat="server" id="formname" name="formname" />
            <input type="hidden" runat="server" name="uid" id="uid" />
            <table style="border: 0px">
                <tbody runat="server" id="entrybody">
                </tbody>
            </table>
            <div>
                <p>
                    <input type="submit" value="<%= ResourceManager.GetLiteral("Admin.Common.Save")%>"
                        id="savebutton" />
                    <input type="reset" value="<%= ResourceManager.GetLiteral("Admin.Common.Cancel")%>"
                        id="cancelbutton" />
                </p>
            </div>
            </form>
        </div>
    </div>
</asp:Content>
