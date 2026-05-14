namespace Pacs008_Validator.Validation.Business.Rules
{
    public sealed class DemoRule : IValidationRule
    {
        private readonly ILogger<DemoRule> _logger;

        public DemoRule(ILogger<DemoRule> logger) => _logger = logger;

        public string Code => "DEMO";
        public string Name => "Demo / pipeline sanity check";
        public int Order => 1;

        public Task<RuleOutcome> ExecuteAsync(
            ValidationContext context,
            ValidationResult result,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "DemoRule ran. Found {Count} transaction(s).",
                context.Transactions.Count);
            return Task.FromResult(RuleOutcome.Continue);
        }
    }
}
