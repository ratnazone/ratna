<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="login.aspx.cs" Inherits="Jardalu.Ratna.Web.Admin.Login" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title><%= ResourceManager.GetLiteral("Login.Title")%></title>
    <link rel="stylesheet" href="template/css/960.css" type="text/css" />
    <link rel="stylesheet" href="template/css/template.css" type="text/css" />
    <link rel="stylesheet" href="template/css/colour.css" type="text/css" />
    <script type="text/javascript" src='<%=ResolveUrl("~/external/jquery/1.6.2/jquery-1.6.2.min.js")%>'></script>
    <script type="text/javascript" src='<%=ResolveUrl("~/scripts/common.js")%>'></script>
    <script type="text/javascript" src='<%=ResolveUrl("~/scripts/converter.js")%>'></script>
    <script type="text/javascript" src='<%=ResolveUrl("~/scripts/crypto/crypto-256.js")%>'></script>
    <script type="text/javascript" src='<%=ResolveUrl("~/scripts/encoders/base64.js")%>'></script>
    <script type="text/javascript" src='<%=ResolveUrl("~/scripts/encoders/UTF8.js")%>'></script>
    <script language="javascript" type="text/javascript">
        $(document).ready(function () {
            SetMainDivAtCenter();
            Setup();
        });

        $(window).resize(function () {
            SetMainDivAtCenter();
        });

        function Setup() {
            $("#loginbutton").click(function () {
                OnLoginClick();
            });
        }

        function OnLoginClick() {
            var password = $("#password").val();

            if (password == null || password == "") {
                return;
            }

            var passwordBytes = UTF8.encode(password);

            //create the sha256 bytes
            var passwordHash = Crypto.SHA256(passwordBytes, { asString: true });
            var hash64 = Base64.encode(passwordHash);

            $("#passwordhash").val(hash64);
            $("#loginform").submit();
        }
    </script>
</head>
<body style="background: white">
    <form id="loginform" method="post" action="login.aspx">
    <div class="login" id="mainDiv">
        <div style="margin-bottom: 15px">
            <img alt="logo" id="logo" src="/images/logo.png"
                title="logo" />
        </div>
        <div id="errordiv" runat="server">
            <p class="error" id="error" runat="server"></p>
        </div>
        <div>
            <label for="name">
                <asp:Literal runat="server" Text="<%$ Resource:Login.Username%>" /></label>
            <input type="text" name="username" value="" />
        </div>
        <div>
            <label for="password">
                <asp:Literal runat="server" Text="<%$ Resource:Login.Password%>" /></label>
            <input type="password" id="password" value="" />
        </div>
        <div>
            <fieldset>
                <div>
                    <label for="rememberme" style="float: right; display: inline-block">
                        <small><asp:Literal runat="server" Text="<%$ Resource:Login.RememberMe%>" /></small></label>
                    <input type="checkbox" name="remember" style="float: right; display: inline-block" />
                </div>
            </fieldset>
        </div>
        <div>
            <input type="hidden" name="r" id="r" value="" runat="server"/>
            <input type="hidden" name="password" id="passwordhash" value="" />
            <input type="submit" value="<%= ResourceManager.GetLiteral("Login.Login")%>" id="loginbutton" />
        </div>
    </div>
    </form>
</body>
</html>
