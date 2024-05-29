using Microsoft.AspNetCore.Mvc;

namespace api.Controllers;

[ApiController]
[Route("api/v1/status")]
public class StatusController : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(200)]
    public IActionResult GetStatus()
    {
        return Ok("Service is running");
    }
}