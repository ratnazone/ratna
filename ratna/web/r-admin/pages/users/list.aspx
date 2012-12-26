<%@ Page Title="" Language="C#" MasterPageFile="~/r-admin/template/admin.Master" AutoEventWireup="true" 
CodeBehind="list.aspx.cs" Inherits="Jardalu.Ratna.Web.Admin.users.list" %>

<%@ Register TagPrefix="Ratna" TagName="PageAccess" Src="~/Controls/Common/PageAccess.ascx" %>
<%@ Register TagPrefix="Admin" TagName="UsersList" Src="~/r-admin/controls/Users/UsersList.ascx" %>
<%@ Register TagPrefix="Ratna" TagName="SimplePager" Src="~/Controls/Pagers/SimplePager.ascx" %>

<asp:Content ID="head" ContentPlaceHolderID="head" runat="server">
    <Ratna:PageAccess id="pageAccess" runat="server" IsRestricted="true" AllowGroups="Administrator"/>
    <title><%= ResourceManager.GetLiteral("Admin.Users.Title")%></title>

    <script type="text/javascript" src='<%=ResolveUrl("~/scripts/post.js")%>'></script>
    <script type="text/javascript" src='<%=ResolveUrl("~/scripts/notification.js")%>'></script>

    <script language="javascript" type="text/javascript">
        $(document).ready(function () {
            $("#searchbutton").click(function () {
                OnSearchClick();
            });
        });

        function OnSearchClick() {
            var query = $("#searchquery").val();
            window.location = "<%=Request.Url.AbsolutePath%>?type=<%=RequestUserType%>&q={0}".formatEscape(query);
        }
        
    </script>

</asp:Content>
<asp:Content ID="mainContent" ContentPlaceHolderID="mainContentPlaceHolder" runat="server">
   
   <div id="content" class="container_16 clearfix">

   <h1 id="headerH1" runat="server" class="grid_16"></h1>
   
   <!-- search option -->
   <div id="searchdiv" style="margin-bottom:10px">
        <input type="text" id="searchquery" />
        <input type="button" value="<%$ Resource:Admin.Common.Search %>" id="searchbutton" runat="server"/>
   </div>

   <!-- user list goes here -->
   <Admin:UsersList id="usersList" runat="server" ExpandedView="true" DisplayHeader="false" DisplayTableHeader="true"/>

   <!-- pagination goes here -->
   <Ratna:SimplePager id="Pager" runat="server" numberofpages="10" pagesize="10" />

   </div>

</asp:Content>
