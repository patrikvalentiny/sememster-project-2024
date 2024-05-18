using api.Controllers;
using AutoFixture;
using FluentAssertions;
using infrastructure.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;
using service;

namespace unit_tests;

public class Tests
{
    private readonly IMock<IDeviceService> _deviceService = new Mock<IDeviceService>();
    private readonly IMock<IConfigService> _configService = new Mock<IConfigService>();
    private readonly IMock<IMotorService> _motorService = new Mock<IMotorService>();
    private readonly IFixture _fixture = new Fixture();
    

    [Test]
    public void StatusResponseTest()
    {
        var controller = new StatusController();
        var result = controller.GetStatus();

        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult!.Value.Should().Be("Service is running");
        okResult.StatusCode.Should().Be(200);
    }
    
    [Test]
    public void GetDevicesTest()
    {
        var deviceService = new Mock<IDeviceService>();
        var data = _fixture.CreateMany<Device>(3);
        deviceService.Setup(x => x.GetDevices()).Returns(data);
        
        var controller = new DeviceController(deviceService.Object, _configService.Object, _motorService.Object);
        var result = controller.GetDevices();
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult!.Value.Should().Be(data);
    }
    
    // Make sure to test the GetDeviceTest returns bad request when an exception is thrown
    [Test]
    public void GetDevicesTestThrows404()
    {
        var dataService = new Mock<IDeviceService>();
        dataService.Setup(x => x.GetDevices()).Throws(new Exception());
        
        var controller = new DeviceController(dataService.Object, _configService.Object, _motorService.Object);
        var result = controller.GetDevices();
        result.Should().BeOfType<BadRequestObjectResult>();
    }
    
    [Test]
    public void GetDeviceConfigTest()
    {
        var configService = new Mock<IConfigService>();
        var fixture = new Fixture();
        var data = fixture.Create<DeviceConfig>();
        configService.Setup(x => x.GetDeviceConfig(It.IsAny<string>())).Returns(data);
        
        var controller = new DeviceController(_deviceService.Object, configService.Object, _motorService.Object);
        var result = controller.GetDeviceConfig("mac");
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult!.Value.Should().Be(data);
    }
    
    // Make sure to test the GetDeviceConfigTest returns bad request when an exception is thrown
    [Test]
    public void GetDeviceConfigTestThrows404()
    {
        var configService = new Mock<IConfigService>();
        configService.Setup(x => x.GetDeviceConfig(It.IsAny<string>())).Throws(new Exception());
        
        var controller = new DeviceController(_deviceService.Object, configService.Object, _motorService.Object);
        var result = controller.GetDeviceConfig("mac");
        result.Should().BeOfType<BadRequestObjectResult>();
    }
}