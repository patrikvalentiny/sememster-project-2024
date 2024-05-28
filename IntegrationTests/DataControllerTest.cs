using System.Data.Common;
using System.Net;
using Dapper;
using FluentAssertions;
using infrastructure.Models;
using Newtonsoft.Json;

namespace IntegrationTests;

public class DataControllerTest
{
    private readonly HttpClient _client = new();
    
    [SetUp]
    public void Setup()
    {
        Helper.RebuildDb();
    }
    
    [Test]
    public async Task GetData()
    {
        //arrange
        var sql = @"
            INSERT INTO climate_ctrl.devices (mac)
            VALUES ('123456789012');
            INSERT INTO climate_ctrl.device_config (mac, last_motor_position, max_motor_position, motor_reversed)
            VALUES ('123456789012', 0, 100, false);
            INSERT INTO climate_ctrl.bme_data (device_mac, temperature_c, pressure, humidity)
            VALUES ('123456789012', 25.0, 1000.0, 50.0),
                   ('123456789012', 25.0, 1000.0, 50.0),
                   ('123456789012', 25.0, 1000.0, 50.0),
                   ('123456789012', 25.0, 1000.0, 50.0),
                   ('123456789012', 25.0, 1000.0, 50.0),
                   ('123456789012', 25.0, 1000.0, 50.0),
                   ('123456789012', 25.0, 1000.0, 50.0),
                   ('123456789012', 25.0, 1000.0, 50.0),
                   ('123456789012', 25.0, 1000.0, 50.0),
                   ('123456789012', 25.0, 1000.0, 50.0);";
        await using var conn = Helper.OpenConnection();
        await conn.ExecuteAsync(sql);


        //act
        var response = await _client.GetAsync(Helper.BaseUrl + "/data/123456789012?days=1");
        var content = JsonConvert.DeserializeObject<BmeDataDto[]>(await response.Content.ReadAsStringAsync());
        
        //assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        content.Should().NotBeNullOrEmpty();
        content.Should().HaveCount(10);
        content.Should().OnlyContain(x => Math.Abs(x.Humidity - 50.0) < 1);
        content.Should().OnlyContain(x => Math.Abs(x.TemperatureC - 25.0) < 1);
        content.Should().OnlyContain(x => Math.Abs(x.Pressure - 1000.0) < 1);
    }
    
    [Test]
    public async Task GetDataNoData()
    {
        //act
        var response = await _client.GetAsync(Helper.BaseUrl + "/data/123456789012?days=1");
        var content = JsonConvert.DeserializeObject<BmeDataDto[]>(await response.Content.ReadAsStringAsync());
        //assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        content.Should().BeEmpty();
    }
    
    [TestCase("12345678901")]
    [TestCase("1234567890123")]
    [TestCase("mac")]
    public async Task GetDataWrongMac(string mac)
    {
        //act
        var response = await _client.GetAsync(Helper.BaseUrl + $"/data/{mac}?days=1");
        //assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
    
    [TestCase(0)]
    [TestCase(-1)]
    [TestCase(10000)]
    public async Task GetDataWrongDays(int days)
    {
        //act
        var response = await _client.GetAsync(Helper.BaseUrl + $"/data/123456789012?days={days}");
        //assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
    
}