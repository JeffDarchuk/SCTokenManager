using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sitecore.Data.Items;
using TokenManager.Collections;
using TokenManager.Data.Interfaces;
using TokenManager.Data.Tokens;

namespace TokenTestProject.DynamicTokens
{
	[TestClass]
	public class DynamicTokenTest
	{
		public class TestTokenCollection : TokenCollection<IToken>
		{
			public TestTokenCollection(IEnumerable<IToken> tokens) : base(tokens)
			{
			}

			public override IToken InitiateToken(string token)
			{
				throw new NotImplementedException();
			}

			public override string GetCollectionLabel()
			{
				return "test";
			}

			public override void ResetTokenCache()
			{
				throw new NotImplementedException();
			}
		}
		private IEnumerable<IToken> GetDemoTokens()
		{
			yield return new DynamicToken("test1", () => "test1TokenValue");
		}
		[TestMethod]
		public void BuildDynamicTokens()
		{
			var tc = new TestTokenCollection(GetDemoTokens());
			Assert.AreEqual("test1TokenValue", tc["test1"].Value(null));
		}
	}
}
