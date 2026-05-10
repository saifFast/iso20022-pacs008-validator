namespace Pacs008_Validator.Application.Validation
{
    public sealed record XsdValidationError(
      string Severity,    // "Error" or "Warning"
      string Message,
      int LineNumber,
      int LinePosition);
}
