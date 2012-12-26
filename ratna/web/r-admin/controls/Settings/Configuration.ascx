<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Configuration.ascx.cs" 
    Inherits="Jardalu.Ratna.Web.Admin.controls.Settings.Configuration" %>

<%@ Register TagPrefix="Ratna" TagName="ClientJavaScript" Src="~/Controls/Common/ClientJavaScript.ascx" %>

<Ratna:ClientJavaScript id="clientJavaScript" runat="server" />

<div class="box">

    <span id="headerspan" runat="server">
        <h2><%= ResourceManager.GetLiteral("Admin.Settings.Configuration.Header")%></h2>
    </span>

    <table>
        <tbody>
            <tr>
                <td>
                    <label><%= ResourceManager.GetLiteral("Admin.Settings.Configuration.Logging")%></label>
                </td>
                <td>
                    <select id="loggingSelect" runat="server" />
                </td>
                <td>
                    <label><%= ResourceManager.GetLiteral("Admin.Settings.Configuration.LoggingLevel")%></label>
                </td>
                <td>
                    <select id="loggingLevelSelect" runat="server" />
                </td>
            </tr>
            <tr>
                <td colspan="4" style="text-align:right">
                    <input type="button" id="configurationSaveButton" runat="server" />
                </td>
            </tr>
        </tbody>
    </table>

</div>
