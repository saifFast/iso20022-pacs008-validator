using Microsoft.AspNetCore.Mvc;

namespace Pacs008_Validator.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Pacs008_Validator.Application.Validation;
    using System.Text;


    [ApiController]
    [Route("api/validation")]
    public class ValidationController : ControllerBase
    {
        private readonly XsdValidator _validator;

        public ValidationController(XsdValidator validator) => _validator = validator;

        [HttpPost("pacs008/xsd")]
        [Consumes("application/xml", "text/xml")]
        [Produces("application/json")]
        public async Task<IActionResult> ValidateXsd(CancellationToken ct)
        {
            using var reader = new StreamReader(Request.Body, Encoding.UTF8);
            var xml = await reader.ReadToEndAsync(ct);

            var result = _validator.Validate(xml);
            return Ok(result);
        }
    }
}
