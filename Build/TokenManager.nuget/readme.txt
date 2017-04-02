TOKENMANAGER README

Thanks for installing TokenManager! Here are some tips to get you started:

Begin by creating a class that extends AutoToken or AutoViewToken if you would like to render a cshtml.

When building an autotoken it's important to note:

1.  You must provide a parameterless constructor that calls to the base implementation constructior giving it 
	the collection name, icon (using the same path snippet as is used in sitecore icons), and token name.
2.  Extra data is the data that will be collected from the user when the token is selected.
	-IdTokenData for Sitecore item
	-StringTokenData for simple strings
	-BooleanTokenData for true/false
	-IntegerTokenData for numbers
	-DroplistTokenData for pick lists
	-GeneralLinkTokenData for links
3.  You can override the TokenButton method to add a button to the RTE ribbon for your token automatically.
4.  You can control where this token is available by overriding
	-ValidTemplates()
	-ValidParents()
	-IsCurrentContextValid(Item item) *note, the item may be null.