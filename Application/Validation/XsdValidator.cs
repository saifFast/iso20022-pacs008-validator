namespace Pacs008_Validator.Application.Validation
{
    using System.Xml;
    using System.Xml.Schema;
    public sealed class XsdValidator
    {
        private const string Pacs008Namespace = "urn:iso:std:iso:20022:tech:xsd:pacs.008.001.08";

        private readonly XmlSchemaSet _schemas;
        private readonly ILogger<XsdValidator> _logger;

        public XsdValidator(IWebHostEnvironment env, ILogger<XsdValidator> logger)
        {
            _logger = logger;

            var schemaPath = Path.Combine(env.ContentRootPath, "Schemas", "pacs.008.001.08.xsd");
            if (!File.Exists(schemaPath))
                throw new FileNotFoundException(
                    $"pacs.008 XSD not found at '{schemaPath}'. " +
                    "Make sure pacs.008.001.08.xsd is in the Schemas folder and copied to output.",
                    schemaPath);

            _schemas = new XmlSchemaSet();
            _schemas.Add(Pacs008Namespace, schemaPath);
            _schemas.Compile();

            _logger.LogInformation("Compiled pacs.008.001.08 schema from {Path}", schemaPath);
        }

        public XsdValidationResult Validate(string xml)
        {
            if (string.IsNullOrWhiteSpace(xml))
            {
                return new XsdValidationResult
                {
                    IsValid = false,
                    Errors = new[]
                    {
                    new XsdValidationError("Error", "Empty request body.", 0, 0)
                }
                };
            }

            var errors = new List<XsdValidationError>();

            var settings = new XmlReaderSettings
            {
                ValidationType = ValidationType.Schema,
                Schemas = _schemas,
                ValidationFlags = XmlSchemaValidationFlags.ReportValidationWarnings
                                | XmlSchemaValidationFlags.ProcessInlineSchema
                                | XmlSchemaValidationFlags.ProcessSchemaLocation,
                DtdProcessing = DtdProcessing.Prohibit,
                XmlResolver = null
            };

            settings.ValidationEventHandler += (_, e) =>
            {
                errors.Add(new XsdValidationError(
                    Severity: e.Severity.ToString(),
                    Message: e.Message,
                    LineNumber: e.Exception?.LineNumber ?? 0,
                    LinePosition: e.Exception?.LinePosition ?? 0));
            };

            try
            {
                using var sr = new StringReader(xml);
                using var reader = XmlReader.Create(sr, settings);
                while (reader.Read()) { /* drain — handler collects errors */ }
            }
            catch (XmlException xex)
            {
                // Malformed XML — parser couldn't even walk the document.
                errors.Add(new XsdValidationError(
                    Severity: "Error",
                    Message: $"XML parse error: {xex.Message}",
                    LineNumber: xex.LineNumber,
                    LinePosition: xex.LinePosition));
            }

            var hasError = errors.Any(e => e.Severity == "Error");
            return new XsdValidationResult
            {
                IsValid = !hasError,
                Errors = errors
            };
        }
    }
}
