<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ProfileSummary.ascx.cs"
    Inherits="Jardalu.Ratna.Web.Admin.controls.ProfileSummary" %>
<div class="box">
    <h2>
        <asp:Literal ID="userName" runat="server" /></h2>
    <div class="utils">
        <a id="profileanchor" href="" runat="server"><%= ResourceManager.GetLiteral("Admin.Common.More")%></a>
    </div>
    <p>
        <img runat="server" id="profilePhoto" src="~/images/gravatar.jpg" style="width: 60px;
            height: 60px; text-align: center" />
        <br />
        <strong><%= ResourceManager.GetLiteral("Admin.Profile.LastSignedIn")%></strong>
        <asp:Literal ID="lastSignedInTime" runat="server" /><br />
    </p>
</div>
