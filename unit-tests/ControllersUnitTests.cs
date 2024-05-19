using api.Controllers;
using AutoFixture;
using commons.Models;
using FluentAssertions;
using infrastructure;
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
        var data = _fixture.Create<DeviceConfig>();
        configService.Setup(x => x.GetDeviceConfig(It.IsAny<string>())).Returns(data);
        
        var controller = new DeviceController(_deviceService.Object, configService.Object, _motorService.Object);
        var result = controller.GetDeviceConfig("mac");
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult!.Value.Should().Be(data);
    }
    
    [Test]
    public void GetDeviceConfigTestThrows404()
    {
        var configService = new Mock<IConfigService>();
        configService.Setup(x => x.GetDeviceConfig(It.IsAny<string>())).Throws(new Exception());
        
        var controller = new DeviceController(_deviceService.Object, configService.Object, _motorService.Object);
        var result = controller.GetDeviceConfig("mac");
        result.Should().BeOfType<BadRequestObjectResult>();
    }
    
    [Test]
    public void GetMotorPositionTest()
    {
        var motorService = new Mock<IMotorService>();
        var data = _fixture.Create<MotorPositionDto>();
        motorService.Setup(x => x.GetMotorPosition(It.IsAny<string>())).ReturnsAsync(data);
        
        var controller = new DeviceController(_deviceService.Object, _configService.Object, motorService.Object);
        var result = controller.GetMotorPosition("mac");
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult!.Value.Should().Be(data);
    }
    
    [Test]
    public void GetMotorPositionTestThrows404()
    {
        var motorService = new Mock<IMotorService>();
        motorService.Setup(x => x.GetMotorPosition(It.IsAny<string>())).Throws(new Exception());
        
        var controller = new DeviceController(_deviceService.Object, _configService.Object, motorService.Object);
        var result = controller.GetMotorPosition("mac");
        result.Should().BeOfType<BadRequestObjectResult>();
    }
    
    [Test]
    public void SetMaxMotorPositionTest()
    {
        var motorService = new Mock<IMotorService>();
        var data = _fixture.Create<int>();
        motorService.Setup(x => x.SetMaxMotorPosition(It.IsAny<string>(), It.IsAny<int>())).ReturnsAsync(data);
        
        var controller = new DeviceController(_deviceService.Object, _configService.Object, motorService.Object);
        var result = controller.SetMaxMotorPosition("mac", data);
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult!.Value.Should().Be(data);
    }
    
    
    [Test]
    public void SetMaxMotorPositionTestThrows404()
    {
        var motorService = new Mock<IMotorService>();
        motorService.Setup(x => x.SetMaxMotorPosition(It.IsAny<string>(), It.IsAny<int>())).Throws(new Exception());
        
        var controller = new DeviceController(_deviceService.Object, _configService.Object, motorService.Object);
        var result = controller.SetMaxMotorPosition("mac", 0);
        result.Should().BeOfType<BadRequestObjectResult>();
    }
    
    [Test]
    public void SetMotorDirectionTest()
    {
        var motorService = new Mock<IMotorService>();
        var data = _fixture.Create<bool>();
        motorService.Setup(x => x.SetMotorDirection(It.IsAny<string>(), It.IsAny<bool>())).ReturnsAsync(data);
        
        var controller = new DeviceController(_deviceService.Object, _configService.Object, motorService.Object);
        var result = controller.SetMotorDirection("mac", data);
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult!.Value.Should().Be(data);
    }
    
    [Test]
    public void SetMotorDirectionTestThrows404()
    {
        var motorService = new Mock<IMotorService>();
        motorService.Setup(x => x.SetMotorDirection(It.IsAny<string>(), It.IsAny<bool>())).Throws(new Exception());
        
        var controller = new DeviceController(_deviceService.Object, _configService.Object, motorService.Object);
        var result = controller.SetMotorDirection("mac", true);
        result.Should().BeOfType<BadRequestObjectResult>();
    }
    
    //get motor direction test
    [Test]
    public void GetMotorDirectionTest()
    {
        var motorService = new Mock<IMotorService>();
        var data = _fixture.Create<bool>();
        motorService.Setup(x => x.GetMotorReversed(It.IsAny<string>())).Returns(data);
        
        var controller = new DeviceController(_deviceService.Object, _configService.Object, motorService.Object);
        var result = controller.GetMotorDirection("mac");
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult!.Value.Should().Be(data);
    }
    
    [Test]
    public void GetMotorDirectionTestThrows404()
    {
        var motorService = new Mock<IMotorService>();
        motorService.Setup(x => x.GetMotorReversed(It.IsAny<string>())).Throws(new Exception());
        
        var controller = new DeviceController(_deviceService.Object, _configService.Object, motorService.Object);
        var result = controller.GetMotorDirection("mac");
        result.Should().BeOfType<BadRequestObjectResult>();
    }
    
    
    [Test]
    public void GetDataTest()
    {
        var dataService = new Mock<IDataService>();
        var data = _fixture.CreateMany<BmeData>(3);
        dataService.Setup(x => x.GetDeviceDataInLastXDays(It.IsAny<string>(), It.IsAny<int>())).Returns(data);
        
        var controller = new DataController(dataService.Object);
        var result = controller.GetData("mac", 3);
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult!.Value.Should().Be(data);
    }
    
    [Test]
    public void GetDataTestThrows404()
    {
        var dataService = new Mock<IDataService>();
        dataService.Setup(x => x.GetDeviceDataInLastXDays(It.IsAny<string>(), It.IsAny<int>())).Throws(new Exception());
        
        var controller = new DataController(dataService.Object);
        var result = controller.GetData("mac", 3);
        result.Should().BeOfType<BadRequestObjectResult>();
    }
    
}