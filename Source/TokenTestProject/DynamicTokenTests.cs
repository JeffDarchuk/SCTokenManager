using System;
using TokenManager.Management;
using System.Collections.Generic;
using TokenManager.Data;
using NUnit.Framework;

namespace TokenTestProject
{
	public class DynamicTokenTests
	{
		TokenKeeper keeper = new TokenKeeper("{{","}}","|");
		[Test]
		public void GetsDynamicKeyLoadedIn()
		{
			//var tst = new DynamicTokenCollection("test", new List<DynamicToken>()
			//{
			//	new DynamicToken("rightnow", ()=>DateTime.Now.ToString()),
			//	new DynamicToken("tomorrow", ()=>DateTime.Now.AddDays(1).ToString())
			//});
			//keeper.LoadTokenGroup(tst);
			//keeper.GetTokenValue("rightnow");
			//Assert.AreEqual(keeper.GetTokenValue("rightnow"), keeper.GetTokenValue("test", "rightnow"));
			//Assert.AreNotEqual(keeper.GetTokenValue("rightnow"), keeper.GetTokenValue("tomorrow"));
		}
	}
}
