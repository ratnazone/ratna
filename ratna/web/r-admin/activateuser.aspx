<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="activateuser.aspx.cs" Inherits="Jardalu.Ratna.Web.Admin.activateuser" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title>Activate user account</title>
    <link rel="stylesheet" href="template/css/960.css" type="text/css" />
    <link rel="stylesheet" href="template/css/template.css" type="text/css" />
    <link rel="stylesheet" href="template/css/colour.css" type="text/css" />
    <script type="text/javascript" src='<%=ResolveUrl("~/external/jquery/1.6.2/jquery-1.6.2.min.js")%>'></script>
    <script type="text/javascript" src='<%=ResolveUrl("~/scripts/common.js")%>'></script>
    <script language="javascript" type="text/javascript">
        $(document).ready(function () {
            SetMainDivAtCenter();
        });

        $(window).resize(function () {
            SetMainDivAtCenter();
        });
    </script>
</head>
<body>
    <div id="mainDiv" style="width:400px">
        <div id="errordiv" runat="server">
            <p class="error">
                Unable to activate the user account.</p>
        </div>
        <div id="successdiv" runat="server">
            <p class="success">
                User has been activated. Please <a href="/r-admin/login.aspx">login</a> to continue.
            </p>
        </div>
    </div>
</body>
</html>
