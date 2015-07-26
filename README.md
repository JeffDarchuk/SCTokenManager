# SCTokenManager
Token Manager for Sitecore


Generic rich text token management system.  Easilly inject dynamic content into rich text fields, that is fully tracked, managed, and analyzed.
# The basics
TokenManager sitecore items are required.  Package here:
https://onedrive.live.com/redir?resid=A4BC37C253DAEF6A!153&authkey=!AGy8WoWZQOS36lE&ithint=file%2czip

A TokenManager sitecore item is added as a child item of sitecore/Content.  

A token is identified by two strings, category name and token name (i.e. Date > Now | Session > User Name).  

each token category has an item representation in the TokenManager item. OOTB there is a dynamic token collection (run code snippet to get value) or general token collection (get value from rich text field).  

Token values are lazy loaded into a cache as well as the locations where they are used in the rich text fields.

# Features
1. Inject the contents of a rich text field into another rich text field on render.
2. Inject dynamically generated content (such as current date) into a rich text field on render.
3. Filter available tokens based on rich text field's item's template or rich text's field's location in the content tree.
4. Sitecore actions propogate appropriately.
  4a. Moved and saved tokens are properly updated everywhere they're used.
  4b. Deleted tokens are replaced by their former value everywhere they were used.
5. Management App to manage and analyze token usage.  Custom MVC and AngularJS app embedded in binary
  5a. Analyze token usage, edit items tokens are used in
  5b. Convert tokens that desynchronize to reincorporate them into the manager
  5c. Incorporate a specific text snippet into a managed token in all rich text fields
  5d. Unzip a token replacing every usage of it with it's value in all rich text fields
6. Custom content search index value added to track tokens
7. Easilly extendable and overridable to add new types of tokens or new behavior for collections of tokens.
