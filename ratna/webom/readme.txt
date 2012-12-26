Snippets
========================================

Snippets are small units of UI that are used to refresh a HTML page area saving the full reload of the HTML page. Server UI components
can support snippet generation. When a call is made by the client, it can request a snippet output with the response. Client tells which
server component is to be used for snippet generation and passes all the required parameters to generate the snippet.


Handlers
========================================

ArticleHandler -- base type that handles article objects
     when a url is served, and the url contains a location of article, it will load the articlehandler
     from the articlehandler, it looks for the "ItemPage" and forwards the request to the ItemPage for render.


ProgrammedPath
========================================
Paths that are programmed by 3rd party code.


Flow
==============================================================================================
SecurityModule   - Checks blocked path access
UrlReWriter      - Request is made, if its an article, it will rewrite the url.
CustomResponder  - At Response, check for failures