using Microsoft.AspNetCore.Mvc;
using service;

namespace api.Controllers;

[ApiController]
[Route("api/v1/dashboard")]
public class DashboardController(DeviceService deviceService) : Controller
{
    [HttpGet]
    [Route("/devices")]
    public IActionResult GetDevices()
    {
        try
        {
            return Ok(deviceService.GetDevices());
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}