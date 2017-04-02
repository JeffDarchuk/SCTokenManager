#Getting Started

This document will focus on getting started using the most common use case for Token Manager
##Install TokenManager

	Using nuget `Install-Package TokenManager` will add the binaries and single configuration patch file to your sitecore environment.  
	Note that if you may need to move your config file if your solution is not set up on your webroot.

##Implement an AutoToken

1.  Create a new class that implements the abstract class AutoToken
2.  Modify constructor to have no parameters and in the base constructor add
	1. Collection Name - The name given to the grouping of tokens e.x. Search Tokens
	2. Icon - Sitecore Icon for this token, use the same format as Sitecore Items have in fields.
	3. Token Name - The name for the specific token e.x. Results Number
3.  Implement ExtraData method.  This is data that will be collected from the content authors when a token is placed.  It is unique per usage of the token
	and there are a number a data types that can be collected.  Each type defined has a key or name that is used to retrieve the value of the field.
	1. IdTokenData for Sitecore item
	1. StringTokenData for simple strings
	1. BooleanTokenData for true/false
	1. IntegerTokenData for numbers
	1. DroplistTokenData for pick lists
	1. GeneralLinkTokenData for links
4.  Implement Value method.  This is what gets injected into the rendered RTE.  Use the extra data values from the TokenDataCollection with the keys defined
	in step 3 to render the appropriate token markup.
