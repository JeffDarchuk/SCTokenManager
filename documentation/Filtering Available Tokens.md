#Filtering Tokens

For tokens applicable to only certain circumstances a filter can be applied in one of three methods that are overridable in the AutoToken abstract class.
	1. ValidTemplates() - Used to define a set of templates this token may be used on.
	2. ValidParents() - Used to define a set of subtree root nodes that all children under the these nodes are able to use this token.
	3. IsCurrentContextValid(Item item) *note, the item may be null.  Usable to define special parameters for filtering.