using Microsoft.AspNetCore.Mvc;
using service;

namespace api.Controllers;

[ApiController]
[Route("api/v1/status")]
public class StatusController : Controller
{
    
    [HttpGet]
    public IActionResult GetStatus()
    {
        return Ok("Service is running");
    }
}