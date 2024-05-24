using Microsoft.AspNetCore.Mvc;
using Serilog;
using service;

namespace api.Controllers;

[ApiController]
[Route("api/v1/device")]
public class DeviceController(IDeviceService deviceService)
    : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(200)]
    public IActionResult GetDevices()
    {
        try
        {
            return Ok(deviceService.GetDevices());
        }
        catch (Exception e)
        {
            Log.Error(e, "Error getting devices");
            return BadRequest(e.Message);
        }
    }

    [HttpGet("{mac}/config")]
    [ProducesResponseType(200)]
    public IActionResult GetDeviceConfig(string mac)
    {
        try
        {
            return Ok(deviceService.GetDeviceConfig(mac));
        }
        catch (Exception e)
        {
            Log.Error(e, "Error getting device config");
            return BadRequest(e.Message);
        }
    }
}