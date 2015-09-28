using TokenManager.Data.Interfaces;

namespace TokenManager.Data
{
    class BasicTokenData : ITokenData
    {
        public BasicTokenData(string name, string label, string placeholder, bool required, TokenDataType type)
        {
            Name = name;
            Label = label;
            Placeholder = placeholder;
            Required = required;
            Type = type;
        }
        public string Name { get; set; }
        public string Label { get; set; }
        public string Placeholder { get; set; }
        public bool Required { get; set; }
        public TokenDataType Type { get; set; }
        public string Value { get; set; }
    }
}
