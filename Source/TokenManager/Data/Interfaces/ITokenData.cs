namespace TokenManager.Data.Interfaces
{
	public interface ITokenData
	{
		string Name { get; set; }
		string Label { get; set; }
		bool Required { get; set; }
		string AngularMarkup { get;}
		dynamic Data { get;}
		object GetValue(string value);
	}
}