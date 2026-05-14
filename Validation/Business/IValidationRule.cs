namespace Pacs008_Validator.Validation.Business
{
    public interface IValidationRule
    {
        string Code { get; }
        string Name { get; }
        int Order => 100;

        Task<RuleOutcome> ExecuteAsync(
            ValidationContext context,
            ValidationResult result,
            CancellationToken cancellationToken);
    }
}
