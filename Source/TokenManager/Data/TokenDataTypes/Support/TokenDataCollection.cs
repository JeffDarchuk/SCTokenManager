using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Sitecore.Data;

namespace TokenManager.Data.TokenDataTypes.Support
{
	public class TokenDataCollection
	{
		private NameValueCollection _source;

		public string this[string tokenDataName]
		{
			get { return _source[tokenDataName]; }
			set { _source[tokenDataName] = value; }
		}

		public TokenDataCollection(NameValueCollection source)
		{
			_source = source;
		}

		public bool GetBoolean(string tokenDataName)
		{
			return _source[tokenDataName] == "True";
		}

		public GeneralLink GetLink(string tokenDataName)
		{
			return new GeneralLink(_source[tokenDataName]);
		}

		public int GetInt(string tokenDataName)
		{
			int ret;
			if (int.TryParse(_source[tokenDataName], out ret))
			{
				return ret;
			}
			return -1;
		}

		public ID GetId(string tokenDataName)
		{
			string val = _source[tokenDataName];
			if (val == null) return null;
			try
			{
				return new ID(_source[tokenDataName]);
			}
			catch (FormatException e)
			{
				return null;
			}
		}

		public string GetString(string tokenDataName)
		{
			return _source[tokenDataName];
		}

		public string GetDropdownValue(string tokenDataName)
		{
			return _source[tokenDataName];
		}

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			foreach (string key in _source.Keys)
			{
				sb.Append($"{key}={_source[key]}&");
			}
			sb.Remove(sb.Length - 1, 1);
			return sb.ToString();
		}
	}
}
