var scEditor = null;
var scTool = null;

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
	if (scEditor.getSelectedElement().className === "token-manager-token") {
		scEditor.getSelectedElement().outerHTML = returnValue;
	} else {
		scEditor.pasteHtml(returnValue, "DocumentManager");
	}
}

function getToken(editor) {
	if (editor.getSelectedElement().className === "token-manager-token") {
		return editor.getSelectedElement().outerHTML;
	}
}