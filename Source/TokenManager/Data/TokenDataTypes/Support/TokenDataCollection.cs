using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Sitecore.Data;
using Sitecore.Diagnostics;

namespace TokenManager.Data.TokenDataTypes.Support
{
	public class TokenDataCollection
	{
		private NameValueCollection _source;

		public string[] AllKeys
		{
			get { return _source.AllKeys; }
		}

		public void Add(string name, string value)
		{
			_source.Add(name, System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(value)));
		}

		public void Remove(string name)
		{
			_source.Remove(name);
		}

		public IEnumerable<T> Cast<T>()
		{
			return _source.Cast<T>();
		}
		public string this[string tokenDataName]
		{
			get
			{
				string ret = _source[tokenDataName];
				if (string.IsNullOrWhiteSpace(ret))
					return ret;
				try
				{
					return System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String(ret));
				}
				catch (Exception e)
				{
					Log.Warn("Unable to process token data field "+tokenDataName +" because it is not valid base64", e, this);
					return "";
				}
			}
			set { _source[tokenDataName] = System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(value)); }
		}

		public TokenDataCollection()
		{
			_source = new NameValueCollection();
		}
		public TokenDataCollection(NameValueCollection source)
		{
			_source = source;
		}

		public bool GetBoolean(string tokenDataName)
		{
			return this[tokenDataName] == "True";
		}

		public GeneralLink GetLink(string tokenDataName)
		{
			return new GeneralLink(this[tokenDataName]);
		}

		public int GetInt(string tokenDataName)
		{
			int ret;
			if (int.TryParse(this[tokenDataName], out ret))
			{
				return ret;
			}
			return -1;
		}

		public ID GetId(string tokenDataName)
		{
			string val = this[tokenDataName];
			if (val == null) return null;
			try
			{
				return new ID(this[tokenDataName]);
			}
			catch (FormatException e)
			{
				return null;
			}
		}

		public string GetString(string tokenDataName)
		{
			return this[tokenDataName];
		}

		public string GetDropdownValue(string tokenDataName)
		{
			return this[tokenDataName];
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
