namespace Pacs008_Validator.Validation.Business
{
    
    public sealed record ValidationError(
        string RuleCode,
        string ReasonCode,
        string Message,
        string? FieldPath = null,
        string? TxReference = null
    );
}
