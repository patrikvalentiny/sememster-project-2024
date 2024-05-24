using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using service;

namespace api.Controllers;

[ApiController]
[Route("api/v1/data")]
public class DataController(IDataService dataService) : ControllerBase
{
    [HttpGet("{mac}")]
    [ProducesResponseType(200)]
    public IActionResult GetData([Length(12,12)] string mac,[Required][Range(1, 731)][FromQuery] int days = 1)
    {
        try
        {
            return Ok(dataService.GetDeviceDataInLastXDays(mac, days));
        }
        catch (Exception e)
        {
            Log.Error(e, "Error getting data");
            return BadRequest(e.Message);
        }
    }
}