using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Xml;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Globalization;
using Sitecore.Pipelines.RenderField;
using TokenManager.ContentSearch;
using TokenManager.Data.TokenDataTypes.Support;
using TokenManager.Data.Tokens;

namespace TokenManager.Data.Interfaces
{
	public interface ITokenKeeperService
	{
		string TokenPrefix { get; }
		string TokenSuffix { get; }
		string TokenCss { get; set; }
		ITokenCollection<IToken> this[string tokenCollection] { get; set; }

		/// <summary>
		/// Inserts a token collection into the keeper
		/// </summary>
		/// <param name="collection"></param>
		void LoadTokenCollection(ITokenCollection<IToken> collection);

		/// <summary>
		/// Loads up a self contained token instance
		/// </summary>
		/// <param name="token">The token reference</param>
		void LoadAutoToken(AutoToken token);

		/// <summary>
		/// handles replacing the text from a render field pipeline
		/// </summary>
		/// <param name="args"></param>
		/// <param name="text"></param>
		/// <returns></returns>
		string ReplaceRTETokens(RenderFieldArgs args, string text);

		/// <summary>
		/// returns all the tokens in a field
		/// </summary>
		/// <param name="field"></param>
		/// <returns>tokens used in this field</returns>
		IEnumerable<IToken> ParseTokens(Field field, Item item = null);

		/// <summary>
		/// from the field it extracts all the token identifying strings
		/// </summary>
		/// <param name="field"></param>
		/// <returns>token identifying strings</returns>
		IEnumerable<string> ParseTokenIdentifiers(Field field);

		/// <summary>
		/// finds the locations of all the tokens in the field, the first datetime is the modified time of the item
		/// </summary>
		/// <param name="field"></param>
		/// <returns>list of tuples that have two ints that correspond to the token index, and length of the token</returns>
		Tuple<DateTime, List<Tuple<int, int>>> ParseTokenLocations(Field field);

		/// <summary>
		/// given a token identifying text this returns the IToken object
		/// </summary>
		/// <param name="token"></param>
		/// <returns>IToken for the identifier</returns>
		IToken ParseITokenFromText(string token, Item item = null);

		/// <summary>
		/// given a token identifying text this returns the IToken object with an item for checking context validity
		/// </summary>
		/// <param name="props"></param>
		/// <param name="item"></param>
		/// <returns>IToken for the identifier</returns>
		IToken ParseITokenFromProps(TokenDataCollection props, Item item = null);

		/// <summary>
		/// given a token identifier this returns the value of the token with an optional item for checking context validity
		/// </summary>
		/// <param name="token"></param>
		/// <param name="item"></param>
		/// <returns>token value</returns>
		string ParseTokenValueFromTokenIdentifier(string token, Item item = null);

		/// <summary>
		/// based on current token keeper properties it constructs a token identifier
		/// </summary>
		/// <param name="category"></param>
		/// <param name="token"></param>
		/// <returns>token identifier</returns>
		string GetTokenIdentifier(TokenDataCollection data);

		/// <summary>
		/// based on current token keeper properties it constructs a token identifier
		/// </summary>
		/// <param name="category"></param>
		/// <param name="token"></param>
		/// <returns>token identifier</returns>
		string GetTokenIdentifier(string category, string token, dynamic data);

		/// <summary>
		/// based on current token keeper properties it constructs a token identifier
		/// </summary>
		/// <param name="category"></param>
		/// <param name="token"></param>
		/// <returns>token identifier</returns>
		string GetTokenIdentifier(string category, string token, IDictionary<string, object> fields);

		/// <summary>
		/// gets the value of the token belonging to the specific category
		/// </summary>
		/// <param name="category">Category of token</param>
		/// <param name="token">Token name</param>
		/// <param name="extraData">Extra data owned by the token</param>
		/// <returns></returns>
		string GetTokenValue(string category, string token, TokenDataCollection extraData);

		/// <summary>
		/// finds everywhere a specified token is used
		/// </summary>
		/// <param name="category"></param>
		/// <param name="token"></param>
		/// <returns>enumerable of all occurances of this token</returns>
		IEnumerable<ContentSearchTokens> GetTokenOccurances(string category, string token);

		/// <summary>
		/// finds everywhere a specified token is used
		/// </summary>
		/// <param name="category">Category of token</param>
		/// <param name="token">Name of token</param>
		/// <param name="root">root id to get tokens from</param>
		/// <returns>enumerable of all occurances of this token</returns>
		IEnumerable<ContentSearchTokens> GetTokenOccurances(string category, string token, ID root);

		/// <summary>
		/// finds everywhere a specified token is used in a specific database
		/// </summary>
		/// <param name="category">Category of token</param>
		/// <param name="token">Name of token</param>
		/// <param name="db">Database to get token occurances from</param>
		/// <returns>enumerable of all occurances of this token</returns>
		IEnumerable<ContentSearchTokens> GetTokenOccurances(string category, string token, Database db);

		/// <summary>
		/// finds everywhere a specified token is used in a specific database
		/// </summary>
		/// <param name="category">Category of token</param>
		/// <param name="token">Name of token</param>
		/// <param name="db">Database to get token occurances from</param>
		/// <returns>enumerable of all occurances of this token</returns>
		IEnumerable<ContentSearchTokens> GetTokenOccurances(string category, string token, Database db, ID root);

		/// <summary>
		/// finds everywhere a specified token is used in a specific database
		/// </summary>
		/// <param name="category">Category of token</param>
		/// <param name="token">Name of token</param>
		/// <param name="db">Database to get token occurances from</param>
		/// <param name="root">root id to get tokens from</param>
		/// <returns>enumerable of all occurances of this token</returns>
		IEnumerable<ContentSearchTokens> GetTokenOccurances(string category, string token, string db);

		/// <summary>
		/// gets all the labels for the token collections
		/// </summary>
		/// <returns>token collection labels</returns>
		IEnumerable<string> GetTokenCollectionNames();

		/// <summary>
		/// gets all the token collections
		/// </summary>
		/// <returns>token collections</returns>
		IEnumerable<ITokenCollection<IToken>> GetTokenCollections();

		/// <summary>
		/// gets a specific token collection by name
		/// </summary>
		/// <param name="collectionName"></param>
		/// <param name="item"></param>
		/// <returns>token collection</returns>
		ITokenCollection<T> GetTokenCollection<T>(string collectionName, Item item = null)
			where T : IToken;

		/// <summary>
		/// gets a specific token collection by backing ID
		/// </summary>
		/// <param name="collectionName"></param>
		/// <param name="backingItemId"></param>
		/// <param name="item"></param>
		/// <returns>token collection</returns>
		ITokenCollection<T> GetTokenCollection<T>(ID backingItemId, Item item = null)
			where T : IToken;

		/// <summary>
		/// gets all token names under a specific category
		/// </summary>
		/// <param name="category"></param>
		/// <returns></returns>
		IEnumerable<IToken> GetTokens(string category);

		/// <summary>
		/// gets token object from category and token names
		/// </summary>
		/// <param name="category"></param>
		/// <param name="token"></param>
		/// <param name="item"></param>
		/// <returns>token object</returns>
		IToken GetToken(string category, string token, Item item = null);

		/// <summary>
		/// removes to the token collection from the keeper
		/// </summary>
		/// <param name="collectionLabel"></param>
		/// <returns>the collection removed</returns>
		ITokenCollection<IToken> RemoveCollection(string collectionLabel);

		/// <summary>
		/// clears the token location caches for the specific item and field
		/// </summary>
		/// <param name="itemId"></param>
		/// <param name="fieldId"></param>
		void ResetTokenLocations(ID itemId, ID fieldId, Language language, int versionNumber);

		/// <summary>
		/// refreshes the cache for the token collection, if passed in null it refreshes all tokens
		/// </summary>
		/// <param name="collection"></param>
		ITokenCollection<IToken> RefreshTokenCollection(string collection = null);

		/// <summary>
		/// given an item that represents a token collection it will return the collection
		/// </summary>
		/// <param name="item"></param>
		/// <returns>token collection</returns>
		ITokenCollection<IToken> GetCollectionFromItem(Item item);

		/// <summary>
		/// Extracts the token properties out of the token identifier
		/// </summary>
		/// <param name="tokenIdentifier"></param>
		/// <returns></returns>
		TokenDataCollection TokenProperties(string tokenIdentifier);

		/// <summary>
		/// Identifies if the specified range has any part of it inside a token
		/// </summary>
		/// <param name="field"></param>
		/// <param name="startIndex"></param>
		/// <param name="length"></param>
		/// <returns></returns>
		bool IsInToken(Field field, int startIndex, int length);

		/// <summary>
		/// Getting the current context database, or default
		/// </summary>
		/// <returns></returns>
		Database GetDatabase();

		bool HasTokens(Item item);
	}
}
