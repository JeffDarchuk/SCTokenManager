var scEditor = null;
var scTool = null;
var range = null;
//Set the Id of your button into the RadEditorCommandList[]
if (typeof RadEditorCommandList == "undefined")
	RadEditorCommandList = Telerik.Web.UI.Editor.CommandList;
RadEditorCommandList["TokenSelector"] = function (commandName, editor, args) {
	var d = Telerik.Web.UI.Editor.CommandList._getLinkArgument(editor);
	Telerik.Web.UI.Editor.CommandList._getDialogArguments(d, "A", editor, "DocumentManager");

	scEditor = editor;

	var token = getToken(editor);
	editor.showExternalDialog(
        "/TokenManager?sc_itemid=" + scItemID + "&token=" + encodeURIComponent(token),
        null, //argument
        600,
        500,
        scTokenSelectorCallback,
        null,
        "Insert Token",
        true, //modal
        Telerik.Web.UI.WindowBehaviors.Close, // behaviors
        false, //showStatusBar
        false //showTitleBar
        );
};

function scTokenSelectorCallback(sender, returnValue) {
	if (!returnValue || returnValue.text == "") {
		return;
	}
	scEditor.getSelection().selectRange(range);
	if (scEditor.getSelectedElement() != null) {
		var el = scEditor.getSelectedElement();
		el = getWrapper(el);
		if (el && el.className === "token-manager-token") {
			el.outerHTML = returnValue;
			return;
		}
	} 
	scEditor.pasteHtml(returnValue, "DocumentManager");
}

function getToken(editor) {
	range = editor.getSelection().GetRange();
	var el = editor.getSelectedElement();
	el = getWrapper(el);
	if (el && el.outerHTML)
		return el.outerHTML;
	return "";
}
function getWrapper(el) {
	while (el && el.className !== "token-manager-token") {
		el = el.parentElement;
	}
	return el;
}