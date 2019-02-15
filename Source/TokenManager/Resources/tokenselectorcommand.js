if (window.location.href.indexOf("EditorPage.aspx") > -1) {
	var scEditor = null;
	var scTool = null;
	var range = null;
	var tokenElement = null;
	var usingTokenElement = false;
	var tmPreset = new Object();
	var command = null;
//Set the Id of your button into the RadEditorCommandList[]
	if (typeof RadEditorCommandList == "undefined")
		RadEditorCommandList = Telerik.Web.UI.Editor.CommandList;

	RadEditorCommandList["TokenSelector"] = function(commandName, editor, args) {
		var d = Telerik.Web.UI.Editor.CommandList._getLinkArgument(editor);
		Telerik.Web.UI.Editor.CommandList._getDialogArguments(d, "A", editor, "DocumentManager");

		scEditor = editor;
		if (!usingTokenElement)
			tokenElement = null;
		var token = getToken(editor);
		command = commandName;
		jQuery.post("/tokenmanager/tokensetup.json",
			"{'token' : '" +
			token.replace(/</g, "lttt").replace(/>/g, "gttt").replace(/\&/g, 'amppp') +
			"', 'preset' : '" +
			tmPreset[commandName].replace(/</g, "lttt").replace(/>/g, "gttt").replace(/\&/g, 'amppp') +
			"'}").done(function(data) {
			if (data) {
				editor.showExternalDialog(
					"/TokenManager?command="+command+"&sc_itemid=" + scItemID,
					"bannana", //argument
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
			}
		});

	};
	setInterval(function() {
			var els = document.getElementById("Editor_contentIframe").contentWindow.document
				.getElementsByClassName("token-manager-token");
			for (var i = 0; i < els.length; i++) {
				var el = els[i];
				el.onclick = function() {
					usingTokenElement = true;
					var tm = window.parent.document.getElementById('scContentIframeId0').contentWindow.jQuery('.TokenSelector');
					tokenElement = this;
					tm.click();
					usingTokenElement = false;
				}
			}
		},
		500);

	jQuery("[class^='TokenSelector']").each(function(i, el) {
		if (el.className !== "TokenSelector") {
			RadEditorCommandList[el.className] = RadEditorCommandList["TokenSelector"];
			var index = el.parentNode.title.indexOf("(");
			var tokenString =
				el.parentNode.title.substring(el.parentNode.title.indexOf("?") + 1, el.parentNode.title.indexOf(")"));
			tmPreset[el.className] = "href=\"/TokenManager?" + tokenString + "\"";
			el.parentNode.title = el.parentNode.title.substring(0, index);
			jQuery.post("/tokenmanager/tokenvalid.json", { "tokenString": tokenString, "datasource": scItemID }).done(
				function(data) {
					if (!data) {
						el.parentNode.parentNode.style.display = "none";
					}
				});
		} else {
			tmPreset[el.className] = "";
		}
	});
	jQuery.get("/tokenmanager/anytokensvalid.json?sc_itemid=" + scItemID)
		.done(function(data) {
			if (!data) {
				jQuery(".TokenSelector").parent().parent().css("display", "none");
			}
		});
}

function scTokenSelectorCallback(sender, returnValue) {
	if (!returnValue || returnValue.text == "") {
		return;
	}
	if (tokenElement)
		tokenElement.outerHTML = returnValue;
	else {
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
}

function getToken(editor) {
	if (tokenElement)
		return tokenElement.outerHTML;
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