using Microsoft.AspNetCore.Mvc;
using Serilog;
using service;

namespace api.Controllers;

[ApiController]
[Route("api/v1/device")]
public class DeviceController(DeviceService deviceService, ConfigService configService, MotorService motorService) : Controller
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
            Log.Error(e, "Error getting devices");
            throw;
        }
    }
    
    [HttpGet("{mac}/config")]
    public IActionResult GetDeviceConfig(string mac)
    {
        try
        {
            return Ok(configService.GetDeviceConfig(mac));
        }
        catch (Exception e)
        {
            Log.Error(e, "Error getting device config");
            throw;
        }
    }
    
    [HttpGet("{mac}/motor")]
    public IActionResult GetMotorPosition(string mac)
    {
        try
        {
            return Ok(motorService.GetMotorPosition(mac).Result);
        }
        catch (Exception e)
        {
            Log.Error(e, "Error getting motor position");
            throw;
        }
    }
    
    [HttpPut("{mac}/motor")]
    public IActionResult SetMaxMotorPosition(string mac, [FromBody] int position)
    {
        try
        {
            return Ok(motorService.SetMaxMotorPosition(mac, position).Result);
        }
        catch (Exception e)
        {
            Log.Error(e, "Error setting motor position");
            throw;
        }
    }
    
    [HttpPut("{mac}/motor-direction")]
    public IActionResult SetMotorDirection(string mac, [FromBody] bool direction)
    {
        try
        {
            return Ok(motorService.SetMotorDirection(mac, direction).Result);
        }
        catch (Exception e)
        {
            Log.Error(e, "Error setting motor position");
            throw;
        }
    }
    
    [HttpGet("{mac}/motor-direction")]
    public IActionResult GetMotorDirection(string mac)
    {
        try
        {
            return Ok(motorService.GetMotorReversed(mac));
        }
        catch (Exception e)
        {
            Log.Error(e, "Error getting motor position");
            throw;
        }
    }
}