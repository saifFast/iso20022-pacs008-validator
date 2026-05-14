namespace Pacs008_Validator.Validation.Business
{
    public sealed class ValidationResult
    {
        private readonly List<ValidationError> _errors = new();

        public IReadOnlyList<ValidationError> Errors => _errors;
        public bool IsValid => _errors.Count == 0;

        public void AddError(ValidationError error) => _errors.Add(error);

        public void AddError(string ruleCode, string reasonCode, string message,
            string? fieldPath = null, string? txReference = null)
            => _errors.Add(new ValidationError(ruleCode, reasonCode, message, fieldPath, txReference));
    }
}
