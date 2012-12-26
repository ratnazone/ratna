<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="thread.ascx.cs" Inherits="Jardalu.Ratna.Web.templates.system.thread" %>
<%@ Register TagPrefix="Ratna" TagName="ClientJavaScript" Src="~/Controls/Common/ClientJavaScript.ascx" %>
<%@ Register TagPrefix="Ratna" TagName="ThreadMessageRow" Src="messagerow.ascx" %>

<%@ Register TagPrefix="Ratna" TagName="Notification" Src="~/Controls/Common/Notification.ascx" %>

<style type="text/css">
    .notification
    {
        border:1px solid grey;
        background-color: silver;
        padding:4px;
        margin-bottom:10px;
    }
</style>

<script language="javascript" type="text/javascript" src="/scripts/notification.js"></script>
<script language="javascript" type="text/javascript">
    $(document).ready(function () {
        $("#commentformsubmitter").click(function () {
            Notification.Show("#notification", "Saving", false);
            CommentForm.submit("commentform", CommentSubmitted);
        });
    });

    function CommentSubmitted(success, data, formname) {

        // call base method
        CommentForm.OnFormSubmitted(success, data, formname);

        if (success) {
            // parse response

            var response = jQuery.parseJSON(data);
            var message = "";

            if (!response.success) {
                Notification.Show("#notification", "Error saving : " + response.error, false);

            }
            else {            

                //show notification
                Notification.Show("#notification", "Comment saved", true);

                //reset the form
                $("#commentreset").click();
            }
        }
        else {
            // unknown failure
            Notification.Show("#notification", "Unknown failure to submit comment", false);
        }
    }
</script>


<div id="comments" style="margin-top:20px;border-top:2px solid silver">
    <h2><asp:Literal ID="totalCommentsLiteral" runat="server"></asp:Literal>Comments</h2>
    <asp:Repeater ID="repeater" runat="server" OnItemDataBound="RepeaterItemEventHandler">
        <ItemTemplate>
            <Ratna:ThreadMessageRow id="messageRow" runat="server" Name='<%#Eval("Name")%>' 
                Time='<%# Eval("Time")%>' Message='<%# Eval("Body")%>'/>                
        </ItemTemplate>
    </asp:Repeater>
    <div id="comments-bottom-marker" style="display:none"></div>
    <span style="border-top:1px solid silver;margin-top:10px;display:block">&nbsp;</span>
    <Ratna:Notification ID="notification" runat="server" />
    <h2 style="margin-bottom:10px">Write A Comment</h2>
        <form action="javascript:return false;" method="post" id="commentform">
          <input id="key" name="key" value="<%=Key %>" type="hidden" />
          <input id="permalink" name="permalink" value="" type="hidden" runat="server" />
          <input id="threadrenderer" name="threadrenderer" value="" type="hidden" runat="server" />
          <p>
            <input type="text" name="name" id="name" value="" size="22" class="required" >
            <label for="name"><small>Name (required)</small></label>
          </p>
          <p>
            <input type="text" name="email" id="email" value="" size="22" class="required email" >
            <label for="email"><small>Mail (required)</small></label>
          </p>
          <p>
            <textarea name="message" id="message" cols="25" rows="10" class="required safeHtml"></textarea>
            <label for="message" class="hidden"><small>Comment (required)</small></label>
          </p>
          <p>
            <input name="submit" type="submit" id="commentformsubmitter" value="Submit">
            &nbsp;
            <input name="reset" type="reset" id="commentreset" value="Reset">
          </p>
        </form>
</div>
