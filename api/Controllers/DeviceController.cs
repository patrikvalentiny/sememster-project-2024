using Microsoft.AspNetCore.Mvc;
using service;

namespace api.Controllers;

[ApiController]
[Route("api/v1/device")]
public class DeviceController(DeviceService deviceService) : Controller
{
    [HttpGet]
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