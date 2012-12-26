<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SiteConfiguration.ascx.cs" 
    Inherits="Jardalu.Ratna.Web.Admin.controls.Settings.SiteConfiguration" %>

<%@ Register TagPrefix="Ratna" TagName="ClientJavaScript" Src="~/Controls/Common/ClientJavaScript.ascx" %>

<Ratna:ClientJavaScript id="clientJavaScript" runat="server" />

<div class="box">

    <span id="headerspan" runat="server">
        <h2><%= ResourceManager.GetLiteral("Admin.Settings.SiteConfiguration.Header")%></h2>
    </span>

    <table>
        <tbody>
            <tr>
                <td style="width:25%">
                    <label><%= ResourceManager.GetLiteral("Admin.Settings.SiteConfiguration.Locale")%></label>
                </td>
                <td>
                    <select id="localeSelect" runat="server" />
                </td>
                <td style="width:25%">
                    <label><%= ResourceManager.GetLiteral("Admin.Settings.SiteConfiguration.CommentModeration")%></label>
                </td>
                <td>
                    <select id="commentModerationSelect" runat="server" />
                </td>
            </tr>
            <tr>
                <td colspan="4" style="text-align:right">
                    <input type="button" id="siteConfigurationSaveButton" runat="server" />
                </td>
            </tr>
        </tbody>
    </table>

</div>