namespace TokenManager.Data.Interfaces
{
    public interface ITokenData
    {
        string Name { get; set; }
        string Label { get; set; }
        string Placeholder { get; set; }
        bool Required { get; set; }
        TokenDataType Type { get; set; }
    }

    public enum TokenDataType
    {
        String,
        Id,
        Integer,
        Boolean
    }
}