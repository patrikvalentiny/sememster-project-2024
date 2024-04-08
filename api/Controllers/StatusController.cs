using Microsoft.AspNetCore.Mvc;

namespace api.Controllers;
[ApiController]
[Route("api/v1/status")]
public class StatusController: Controller
{
    
    [HttpGet]
    public IActionResult GetStatus()
    {
        return Ok("API is running");
    }
}