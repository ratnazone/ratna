<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="logout.aspx.cs" Inherits="Jardalu.Ratna.Web.Admin.logout" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title><%= ResourceManager.GetLiteral("Logout.Title")%></title>
    <link rel="stylesheet" href="template/css/960.css" type="text/css" />
    <link rel="stylesheet" href="template/css/template.css" type="text/css" />
    <link rel="stylesheet" href="template/css/colour.css" type="text/css" />
    <script type="text/javascript" src='<%=ResolveUrl("~/external/jquery/1.6.2/jquery-1.6.2.min.js")%>'></script>
    <script type="text/javascript" src='<%=ResolveUrl("~/scripts/common.js")%>'></script>
    <script type="text/javascript" src='<%=ResolveUrl("~/scripts/constants.js")%>'></script>
    <script language="javascript" type="text/javascript">
        $(document).ready(function () {
            SetMainDivAtCenter();
        });

        $(window).resize(function () {
            SetMainDivAtCenter();
        });

        setTimeout(function () {
            window.location.href = Constants.AdminUrl;
        }, 2000);     
    </script>
</head>
<body>
    <div id="mainDiv" style="width:400px">
        <asp:Literal runat="server" Text="<%$ Resource:Logout.LogoutSuccess%>" />
    </div>
</body>
</html>
