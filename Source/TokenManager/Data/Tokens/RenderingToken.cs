using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Text;
using System.Web.Mvc;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Mvc.Helpers;
using TokenManager.Data.Interfaces;
using TokenManager.Management;

namespace TokenManager.Data.Tokens
{
    public class RenderingToken : IToken
    {
        private string _token;
        private ID _backingId;

        public RenderingToken(string token, ID backingId)
        {
            _token = token;
            _backingId = backingId;
        }
        public string Token { get { return _token; } }

        public ID Datasource { get; set; }
        public int Test { get; set; }
        public bool AlsoTest { get; set; }

        public IEnumerable<ITokenData> ExtraData()
        {
            yield return new BasicTokenData("Datasource", "Datasource for the rendering", "", true,TokenDataType.Id);
        }

        public ID GetBackingItemId()
        {
            return _backingId;
        }

        public string Value(NameValueCollection extraData)
        {
            InternalLinkField renderingItem = TokenKeeper.CurrentKeeper.GetDatabase().GetItem(_backingId).Fields["Rendering"];
            TextWriter tw = new StringWriter();
            var h = new HtmlHelper(new ViewContext(new ControllerContext(), new FakeView(), new ViewDataDictionary(), new TempDataDictionary(), tw), new ViewPage());
            StringBuilder sb = new StringBuilder();
            if (!string.IsNullOrWhiteSpace(extraData["Datasource"]))
                sb.Append(new SitecoreHelper(h).Rendering(renderingItem.TargetID.ToString(),
                    new { Datasource = extraData["Datasource"] }));
            return sb.ToString();
        }

        public class FakeView : IView
        {
            public void Render(ViewContext viewContext, TextWriter writer)
            {
                throw new InvalidOperationException();
            }
        }
    }

}
