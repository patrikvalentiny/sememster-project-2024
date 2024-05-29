using System.Net;
using Dapper;
using FluentAssertions;
using infrastructure;
using Newtonsoft.Json;

namespace IntegrationTests;

public class MotorControllerTests
{
    private readonly HttpClient _client = new();
    
    [SetUp]
    public void Setup()
    {
        Helper.RebuildDb();
    }
    
    [Test]
    public async Task GetMotorStatus()
    {
        //arrange
        var sql = @"
            INSERT INTO climate_ctrl.devices (mac)
            VALUES ('123456789012');
            INSERT INTO climate_ctrl.device_config (mac, last_motor_position, max_motor_position, motor_reversed)
            VALUES ('123456789012', 0, 100, false);";
        await using var conn = Helper.OpenConnection();
        await conn.ExecuteAsync(sql);

        //act
        var response = await _client.GetAsync(Helper.BaseUrl + "/device/123456789012/motor");
        var content = JsonConvert.DeserializeObject<MotorPositionDto>(await response.Content.ReadAsStringAsync())!;
        
        //assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        content.Should().NotBeNull();
        content.LastMotorPosition.Should().Be(0);
        content.MaxMotorPosition.Should().Be(100);
        content.MotorReversed.Should().BeFalse();
    }
    
    [Test]
    public async Task GetMotorStatusNoData()
    {
        //act
        var response = await _client.GetAsync(Helper.BaseUrl + "/device/123456789012/motor");
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }
}