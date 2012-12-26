<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Thread.ascx.cs" Inherits="Jardalu.Ratna.Web.Controls.Common.Thread" %>
<div class="post-bottom-section">
    <h4>
        <asp:Literal ID="totalCommentsLiteral" runat="server"></asp:Literal>
        comments</h4>
    <div class="right">
        <ol class="commentlist">
            <li class="depth-1">
                <div class="comment-info">
                    <img alt="" src="~/images/gravatar.jpg" class="avatar" height="40" width="40" runat="server" />
                    <cite><a href="index.html">Erwin</a> Says:
                        <br />
                        <span class="comment-data"><a href="#comment-63" title="">January 31st, 2010 at 10:00
                            pm</a></span> </cite>
                </div>
                <div class="comment-text">
                    <p>
                        Comments are great!</p>
                    <div class="reply">
                        <a rel="nofollow" class="comment-reply-link" href="index.html">Reply</a>
                    </div>
                </div>
                <ul class="children">
                    <li class="depth-2">
                        <div class="comment-info">
                            <img alt="" src="~/images/gravatar.jpg" class="avatar" height="40" width="40" runat="server" />
                            <cite><a href="index.html">Erwin</a> Says:
                                <br />
                                <span class="comment-data"><a href="#" title="">January 31st, 2010 at 8:15 pm</a></span>
                            </cite>
                        </div>
                        <div class="comment-text">
                            <p>
                                And here is a threaded comment.</p>
                            <div class="reply">
                                <a rel="nofollow" class="comment-reply-link" href="index.html">Reply</a>
                            </div>
                        </div>
                    </li>
                    <li class="depth-2">
                        <div class="comment-info">
                            <img alt="" src="~/images/gravatar.jpg" class="avatar" height="40" width="40" runat="server" />
                            <cite><a href="index.html">Erwin</a> Says:
                                <br />
                                <span class="comment-data"><a href="#" title="">January 31st, 2010 at 8:15 pm</a></span>
                            </cite>
                        </div>
                        <div class="comment-text">
                            <p>
                                And here is another threaded comment.</p>
                            <div class="reply">
                                <a rel="nofollow" class="comment-reply-link" href="index.html">Reply</a>
                            </div>
                        </div>
                        <ul class="children">
                            <li class="depth-3">
                                <div class="comment-info">
                                    <img alt="" src="~/images/gravatar.jpg" class="avatar" height="40" width="40" runat="server" />
                                    <cite><a href="index.html">Erwin</a> Says:
                                        <br />
                                        <span class="comment-data"><a href="#" title="">January 31st, 2010 at 8:10 pm</a></span>
                                    </cite>
                                </div>
                                <div class="comment-text">
                                    <p>
                                        Threaded comments are cool!</p>
                                    <div class="reply">
                                        <a rel="nofollow" class="comment-reply-link" href="index.html">Reply</a>
                                    </div>
                                </div>
                            </li>
                        </ul>
                    </li>
                </ul>
            </li>
            <li class="thread-alt depth-1">
                <div class="comment-info">
                    <img alt="" src="~/images/gravatar.jpg" class="avatar" height="40" width="40" runat="server" />
                    <cite><a href="index.html">Erwin</a> Says:
                        <br />
                        <span class="comment-data"><a href="#comment-63" title="">January 31st, 2010 at 8:00
                            pm</a></span> </cite>
                </div>
                <div class="comment-text">
                    <p>
                        Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Donec libero. Suspendisse
                        bibendum.</p>
                    <div class="reply">
                        <a rel="nofollow" class="comment-reply-link" href="index.html">Reply</a>
                    </div>
                </div>
                <ul class="children">
                    <li class="depth-2">
                        <div class="comment-info">
                            <img alt="" src="~/images/gravatar.jpg" class="avatar" height="40" width="40" runat="server" />
                            <cite><a href="index.html">Erwin</a> Says:
                                <br />
                                <span class="comment-data"><a href="#" title="">January 31st, 2010 7:35 pm</a></span>
                            </cite>
                        </div>
                        <div class="comment-text">
                            <p>
                                Donec libero. Suspendisse bibendum.</p>
                            <div class="reply">
                                <a rel="nofollow" class="comment-reply-link" href="index.html">Reply</a>
                            </div>
                        </div>
                        <ul class="children">
                            <li class="depth-3">
                                <div class="comment-info">
                                    <img alt="" src="~/images/gravatar.jpg" class="avatar" height="40" width="40" runat="server" />
                                    <cite><a href="index.html">Erwin</a> Says:
                                        <br />
                                        <span class="comment-data"><a href="#" title="">January 31st, 2010 at 7:20 pm</a></span>
                                    </cite>
                                </div>
                                <div class="comment-text">
                                    <p>
                                        Threaded comments are cool!</p>
                                    <div class="reply">
                                        <a rel="nofollow" class="comment-reply-link" href="index.html">Reply</a>
                                    </div>
                                </div>
                            </li>
                        </ul>
                    </li>
                </ul>
            </li>
            <li class="depth-1">
                <div class="comment-info">
                    <img alt="" src="~/images/gravatar.jpg" class="avatar" height="40" width="40" runat="server" />
                    <cite><a href="index.html">Erwin</a> Says:
                        <br />
                        <span class="comment-data"><a href="#comment-63" title="">January 31st, 2010 at 6:08
                            pm</a></span> </cite>
                </div>
                <div class="comment-text">
                    <p>
                        Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Donec libero. Suspendisse
                        bibendum. Cras id urna. Morbi tincidunt, orci ac convallis aliquam, lectus turpis
                        varius lorem, eu posuere nunc justo tempus leo.</p>
                    <div class="reply">
                        <a rel="nofollow" class="comment-reply-link" href="index.html">Reply</a>
                    </div>
                </div>
                <ul class="children">
                    <li class="depth-2">
                        <div class="comment-info">
                            <img alt="" src="~/images/gravatar.jpg" class="avatar" height="40" width="40" runat="server" />
                            <cite><a href="index.html">Erwin</a> Says:
                                <br />
                                <span class="comment-data"><a href="#comment-63" title="">January 31st, 2010 at 6:08
                                    pm</a> </span></cite>
                        </div>
                        <div class="comment-text">
                            <p>
                                Lorem ipsum dolor sit amet, consectetuer adipiscing elit.</p>
                            <div class="reply">
                                <a rel="nofollow" class="comment-reply-link" href="index.html">Reply</a>
                            </div>
                        </div>
                    </li>
                </ul>
            </li>
        </ol>
    </div>
</div>
<div class="post-bottom-section">
    <h4>
        Leave a Reply</h4>
    <div class="right">
        <form action="index.html" method="post" id="commentform">
        <p>
            <label for="name">
                Name (required)</label><br />
            <input id="name" name="name" value="Your Name" type="text" tabindex="1" />
        </p>
        <p>
            <label for="email">
                Email Address (required)</label><br />
            <input id="email" name="email" value="Your Email" type="text" tabindex="2" />
        </p>
        <p>
            <label for="website">
                Website</label><br />
            <input id="website" name="website" value="Your Website" type="text" tabindex="3" />
        </p>
        <p>
            <label for="message">
                Your Message</label><br />
            <textarea id="message" name="message" rows="10" cols="20" tabindex="4"></textarea>
        </p>
        <p class="no-border">
            <input class="button" type="submit" value="Submit Comment" tabindex="5" />
        </p>
        </form>
    </div>
</div>
