using System.Xml.Linq;

namespace Pacs008_Validator.Validation.Business
{
    public sealed class ValidationContext
    {
        public required XDocument Document { get; init; }
        public required XNamespace Namespace { get; init; }
        public required XElement Message { get; init; }
        public required IReadOnlyList<XElement> Transactions { get; init; }
    }
}
