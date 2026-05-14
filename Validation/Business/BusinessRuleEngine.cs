using System.Diagnostics;
using System.Xml.Linq;

namespace Pacs008_Validator.Validation.Business
{
    public sealed class BusinessRuleEngine
    {
        private const string Pacs008Namespace = "urn:iso:std:iso:20022:tech:xsd:pacs.008.001.14";
        private readonly IReadOnlyList<IValidationRule> _orderedRules;
        private readonly ILogger<BusinessRuleEngine> _logger;

        public BusinessRuleEngine(IEnumerable<IValidationRule> rules, ILogger<BusinessRuleEngine> logger)
        {
            _orderedRules = rules.OrderBy(r => r.Order).ThenBy(r => r.Code, StringComparer.Ordinal).ToList();
            _logger = logger;

            _logger.LogInformation(
            "Business rule engine initialized with {Count} rules: {Rules}",
            _orderedRules.Count,
            string.Join(", ", _orderedRules.Select(r => $"{r.Code}@{r.Order}")));
        }

        public async Task<ValidationResult> ExecuteAsync(string xml, CancellationToken ct)
        {
            var result = new ValidationResult();
            var sw = Stopwatch.StartNew();

            XDocument doc;
            try
            {
                doc = XDocument.Parse(xml, LoadOptions.SetLineInfo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to parse XML in business rule engine");
                result.AddError("ENGINE", "FF01", $"XML could not be parsed: {ex.Message}");
                return result;
            }

            var ctx = TryBuildContext(doc, result);
            if (ctx is null) return result;

            foreach (var rule in _orderedRules)
            {
                ct.ThrowIfCancellationRequested();

                try
                {
                    var outcome = await rule.ExecuteAsync(ctx, result, ct);

                    if (outcome == RuleOutcome.ShortCircuit)
                    {
                        _logger.LogWarning(
                            "Rule {Rule} requested short-circuit. Remaining rules will not execute.",
                            rule.Code);
                        break;
                    }
                }
                catch (OperationCanceledException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Rule {Rule} threw unexpectedly", rule.Code);
                    result.AddError(
                        rule.Code,
                        "FF01",
                        $"Rule '{rule.Code}' failed unexpectedly. The message could not be fully validated.");
                }
            }

            sw.Stop();
            _logger.LogInformation(
                "Business rule pipeline completed in {Ms}ms. Errors: {ErrorCount}",
                sw.ElapsedMilliseconds, result.Errors.Count);

            return result;
        }

        private ValidationContext? TryBuildContext(XDocument doc, ValidationResult result)
        {
            XNamespace ns = Pacs008Namespace;

            var message = doc.Root?.Element(ns + "FIToFICstmrCdtTrf");
            if (message is null)
            {
                result.AddError(
                    "ENGINE", "FF01",
                    "Document is missing FIToFICstmrCdtTrf element. Did XSD validation run first?");
                return null;
            }

            var transactions = message.Elements(ns + "CdtTrfTxInf").ToList();

            return new ValidationContext
            {
                Document = doc,
                Namespace = ns,
                Message = message,
                Transactions = transactions
            };
        }
    }
}
