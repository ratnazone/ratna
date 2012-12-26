<%@ Page Title="" Language="C#" MasterPageFile="~/r-admin/template/admin.Master" AutoEventWireup="true" CodeBehind="metadata.aspx.cs" 
    Inherits="Jardalu.Ratna.Web.Admin.pages.articles.metadata" %>

<%@ Register TagPrefix="Ratna" TagName="PageAccess" Src="~/Controls/Common/PageAccess.ascx" %>
<%@ Register TagPrefix="Common" TagName="SavingNotification" Src="~/Controls/Common/SavingNotification.ascx" %>
<%@ Register TagPrefix="Admin" TagName="Menu" Src="~/r-admin/controls/Menu.ascx" %>
<%@ Register TagPrefix="Ratna" TagName="ClientJavaScript" Src="~/Controls/Common/ClientJavaScript.ascx" %>

<asp:Content ID="head" ContentPlaceHolderID="head" runat="server">
    <Ratna:PageAccess id="pageAccess" runat="server" IsRestricted="true" AllowGroups="Administrator|Editor|Author" />
    <script type="text/javascript" src='<%=ResolveUrl("~/scripts/post.js")%>'></script>
    <script type="text/javascript" src='<%=ResolveUrl("~/scripts/savingnotification.js")%>'></script>

    <Ratna:ClientJavaScript id="clientJavaScript" runat="server" />
</asp:Content>

<asp:Content ID="mainContent" ContentPlaceHolderID="mainContentPlaceHolder" runat="server">
    <div id="content" class="container_16 clearfix" runat="server"> 
        <div class="grid_16">
            <h1><asp:Literal id="headerLiteral" runat="server" Text="<%$ Resource:Admin.Articles.Metadata %>" /></h1>
        </div>

        <Common:SavingNotification id="savingNotification" runat="server" ClassName="grid_15"/>

        <div class="grid_3">
            <Admin:Menu id="menu" runat="server" />
        </div>

        <div class="grid_13">

            <input type="hidden" runat="server" id="urlkey" value="" />
            <input type="hidden" runat="server" id="articleview" value="article" />

            <div class="grid_13">
            <p>
                <label for="navigationtab">
                    <%=  ResourceManager.GetLiteral("Admin.Articles.Edit.NavigationTab")%>
                    <small>
                        <%=  ResourceManager.GetLiteral("Admin.Articles.Edit.NavigationTab.Help")%></small>
                </label>
                <input type="text" class="grid_12 noleftmargin" name="navigationtab" id="navigationtab" runat="server" />
            </p>
            </div>

            <div class="grid_13">
            <p>
                <label for="tags">
                    <%=  ResourceManager.GetLiteral("Admin.Articles.Edit.Tags")%>
                    <small>
                        <%=  ResourceManager.GetLiteral("Admin.Articles.Edit.Tags.Help")%></small>
                </label>
                <input type="text" class="grid_12 noleftmargin" name="tags" id="tags" runat="server" />
            </p>
            </div>

            <div class="grid_13">
            <p>
                <label for="description">
                    <%=  ResourceManager.GetLiteral("Admin.Articles.Edit.Description")%>
                    <small>
                        <%=  ResourceManager.GetLiteral("Admin.Articles.Edit.Description.Help")%></small>
                </label>
                <input type="text" class="grid_12 noleftmargin" name="description" id="description" runat="server" />
            </p>
            </div>

            <div class="grid_13" id="summarydiv" runat="server">
                <p>
                    <label for="summary">
                        <%=  ResourceManager.GetLiteral("Admin.Articles.Edit.Summary")%>
                        <small>
                            <%=  ResourceManager.GetLiteral("Admin.Articles.Edit.Summary.Help")%></small>
                    </label>
                    <textarea name="summary" class="grid_12 noleftmargin" rows="5" id="summary" runat="server"></textarea>
                </p>
            </div>

             <div class="grid_13" id="headdiv" runat="server">
                <p>
                    <label for="summary">
                        <%=  ResourceManager.GetLiteral("Admin.Pages.Edit.Head")%>
                        <small>
                            <%=  ResourceManager.GetLiteral("Admin.Pages.Edit.Head.Help")%></small>
                    </label>
                    <textarea name="headtext" class="grid_12 noleftmargin" rows="8" id="headtext" runat="server"></textarea>
                </p>
            </div>

            <div class="grid_13">
            <p>
                <input type="submit" value="<%= ResourceManager.GetLiteral("Admin.Articles.Edit.Save")%>"
                    id="savebutton" />
                <input type="reset" value="<%= ResourceManager.GetLiteral("Admin.Articles.Edit.Cancel")%>"
                    id="cancelbutton" />
            </p>
            </div>

        </div>

    </div>
    <div id="errorDiv" class="container_16 clearfix" runat="server">
        <label>
            <%=  ResourceManager.GetLiteral("Admin.Articles.Edit.NoArticle")%></label>
    </div>
</asp:Content>
