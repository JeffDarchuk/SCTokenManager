# SCTokenManager
Token Manager for Sitecore


Generic rich text token management system.  Easilly inject dynamic content into rich text fields, that is fully tracked, managed, and analyzed.
# How to install
Nuget - PM> Install-Package TokenManager

TokenManager uses sitecore items and a new system to automatically install the sitecore items.  The only requirements is that the DLL be in the website's bin directory and the Tokens.config is in the app_config/include folder.  The rest will be intalled automatically as soon as the app pool starts up.

# How to use it
1. Set up tokens under the TokenManager item content/TokenManager.
2. New icon in default rich text editor opens a form to select a token to inject.

# Common problems
1.  Tokens analytics aren't reporting correctly - Rebuild your search indexes from the sitecore control panel.

# Features
1. Inject the contents of a rich text field into another rich text field on render.
2. Filter available tokens based on rich text field's item's template or rich text's field's location in the content tree.
3. Sitecore actions propogate appropriately.
  1. Moved and saved tokens are properly updated everywhere they're used.
  2. Deleted tokens are replaced by their former value everywhere they were used.
4. Management App to manage and analyze token usage.  Custom MVC and AngularJS app embedded in binary.
  1. Analyze token usage, edit items tokens are used in.
  2. Incorporate a specific text snippet into a managed token in all rich text fields.
  3. Unzip a token replacing every usage of it with it's value in all rich text fields.
5. Custom content search index value added to track tokens.
6. Easilly extendable and overridable to add new types of tokens or new behavior for collections of tokens.

# Out of the Box Tokens
1.  Text token - inject rich text into the rich text field.
2.  Shared link token - inject an anchor tag with a shared link, change it in one location and have all links change automatically.
3.  Rules token - run a sitecore rules engine block to determine what the value of the token injected should be.
4.  MVC Rendering token - inject a rendering or component into a rich text field.
5.  Dynamic token - execute code snippet to get value.
