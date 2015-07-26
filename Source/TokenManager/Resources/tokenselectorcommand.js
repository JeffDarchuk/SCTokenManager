var scEditor = null;
var scTool = null;

//Set the Id of your button into the RadEditorCommandList[]
RadEditorCommandList["TokenSelector"] = function (commandName, editor, args) {
    var d = Telerik.Web.UI.Editor.CommandList._getLinkArgument(editor);
    Telerik.Web.UI.Editor.CommandList._getDialogArguments(d, "A", editor, "DocumentManager");

    //Retrieve the html selected in the editor
    var html = editor.getSelectionHtml();

    scEditor = editor;

    editor.showExternalDialog(
      "/TokenManager?sc_itemid=" + scItemID,
      null, //argument
      500,
      400,
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

    scEditor.pasteHtml(returnValue, "DocumentManager");
}