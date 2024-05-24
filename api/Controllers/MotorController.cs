using Microsoft.AspNetCore.Mvc;
using Serilog;
using service;

namespace api.Controllers;

[ApiController]
[Route("api/v1/device")]
public class MotorController(IMotorService motorService) : ControllerBase
{
    [HttpGet("{mac}/motor")]
    [ProducesResponseType(200)]
    public IActionResult GetMotorPosition(string mac)
    {
        try
        {
            return Ok(motorService.GetMotorPosition(mac).Result);
        }
        catch (Exception e)
        {
            Log.Error(e, "Error getting motor position");
            return BadRequest(e.Message);
        }
    }

    [HttpPut("{mac}/motor")]
    [ProducesResponseType(200)]
    public IActionResult SetMaxMotorPosition(string mac, [FromBody] int position)
    {
        try
        {
            return Ok(motorService.SetMaxMotorPosition(mac, position).Result);
        }
        catch (Exception e)
        {
            Log.Error(e, "Error setting motor position");
            return BadRequest(e.Message);
        }
    }

    [HttpPut("{mac}/motor-direction")]
    [ProducesResponseType(200)]
    public IActionResult SetMotorDirection(string mac, [FromBody] bool direction)
    {
        try
        {
            return Ok(motorService.SetMotorDirection(mac, direction).Result);
        }
        catch (Exception e)
        {
            Log.Error(e, "Error setting motor position");
            return BadRequest(e.Message);
        }
    }

    [HttpGet("{mac}/motor-direction")]
    [ProducesResponseType(200)]
    public IActionResult GetMotorDirection(string mac)
    {
        try
        {
            return Ok(motorService.GetMotorReversed(mac));
        }
        catch (Exception e)
        {
            Log.Error(e, "Error getting motor position");
            return BadRequest(e.Message);
        }
    }
}