namespace Pacs008_Validator.Controllers
{
    using Microsoft.AspNetCore.Mvc;

    [ApiController]
    [Route("api/validation")]
    public class ValidationController : ControllerBase
    {
        private const string SupportedMsgType = "pacs008";

        private readonly XsdValidator _validator;

        public ValidationController(XsdValidator validator) => _validator = validator;

        [HttpPost("xsd")]
        [Consumes("application/json")]
        [Produces("application/json")]
        public IActionResult ValidateXsd([FromBody] ValidateRequest request)
        {
            if (request is null)
                return BadRequest(new { error = "Request body is required." });

            if (string.IsNullOrWhiteSpace(request.MsgType))
                return BadRequest(new { error = "'msgType' is required." });

            if (!string.Equals(request.MsgType, SupportedMsgType, StringComparison.OrdinalIgnoreCase))
                return BadRequest(new
                {
                    error = $"Unsupported msgType '{request.MsgType}'. Supported: '{SupportedMsgType}'."
                });

            if (string.IsNullOrWhiteSpace(request.Message))
                return BadRequest(new { error = "'message' is required and must contain the pacs.008 XML." });

            var result = _validator.Validate(request.Message);
            return Ok(result);
        }
    }
}
