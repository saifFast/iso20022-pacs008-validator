namespace Pacs008_Validator.Application.Validation
{
    public sealed class XsdValidationResult
    {
        public bool IsValid { get; init; }
        public IReadOnlyList<XsdValidationError> Errors { get; init; } = Array.Empty<XsdValidationError>();
    }
}
