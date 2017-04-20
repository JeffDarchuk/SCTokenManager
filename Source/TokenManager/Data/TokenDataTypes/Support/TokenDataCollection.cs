using System;
using System.Collections.Specialized;
using System.Text;
using Sitecore;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Items;

namespace TokenManager.Data.TokenDataTypes.Support
{
	public class TokenDataCollection
	{
		private readonly NameValueCollection _source;

		public string this[string tokenDataName]
		{
			get => _source[tokenDataName];
			set => _source[tokenDataName] = value;
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

		public Item GetItem(string name)
		{
			var db = Context.ContentDatabase ?? Context.Database ?? Factory.GetDatabase("master");

			var value = this[name];

			if (string.IsNullOrWhiteSpace(value)) return null;

			Item item = db?.GetItem(value);

			return item;
		}

		public MediaItem GetMedia(string name)
		{
			var db = Context.ContentDatabase ?? Context.Database ?? Factory.GetDatabase("master");

			var value = this[name];

			if (string.IsNullOrWhiteSpace(value)) return null;

			MediaItem item = db?.GetItem(value);

			return item;
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
			catch (FormatException)
			{
				return null;
			}
		}

		public string GetString(string tokenDataName)
		{
			return _source[tokenDataName];
		}

		public string GetString(string tokenDataName, string defaultValue)
		{
			var value = _source[tokenDataName];

			if (string.IsNullOrEmpty(value)) return defaultValue;

			return value;
		}

		public string GetDroplistValue(string tokenDataName)
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
