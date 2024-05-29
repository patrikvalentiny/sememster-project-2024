using api;
using api.ClientEventHandlers;
using AutoFixture;
using AutoFixture.AutoMoq;
using commons.Models;
using FluentAssertions;
using infrastructure.Mqtt;
using Microsoft.AspNetCore.Builder;
using Moq;
using service;

namespace UnitTests;

public class HandlersTests
{
    // Test for ClientStartsListeningToDeviceHandler
    [TestCase]
    [TestCase(true)]
    public async Task ClientStartsListeningToDeviceHandlerTest(bool set = false)
    {
        var fixture = new Fixture().Customize(new AutoMoqCustomization());
        var dto = fixture.Create<ClientStartsListeningToDeviceDto>();
        var data = fixture.CreateMany<BmeData>(10).ToList();
        var dataServiceMock = new Mock<IDataService>();
        dataServiceMock.Setup(x => x.GetLatestData(It.IsAny<string>())).Returns(data);
        var mqttMock = new Mock<MqttDeviceCommandsRepository>();
        var stateService = new WebSocketStateService(mqttMock.Object);
        if (set) stateService.MacToConnectionId.TryAdd(dto.Mac, []);
        var handler = new ClientStartsListeningToDevice(stateService, dataServiceMock.Object);
        var result = await handler.Handle(dto, default);
        result.Mac.Should().Be(dto.Mac);
        result.Data.Should().BeEquivalentTo(data);
    }

    [Test]
    public void ClientSaysHelloHandlerTest()
    {
        const string message = "message";
        var handler = new ClientSaysHelloHandler();
        var dto = new ClientSaysHelloDto { Message = message };
        var result = handler.Handle(dto, default).Result;
        result.Message.Should().Be($"Hello, {message}!");
    }

    [TestCase]
    [TestCase(true)]
    public async Task ClientStartsListeningToMotor(bool set = false)
    {
        var fixture = new Fixture().Customize(new AutoMoqCustomization());
        var dto = fixture.Create<ClientStartsListeningToMotorDto>();
        var mqttMock = new Mock<MqttDeviceCommandsRepository>();
        var stateService = new WebSocketStateService(mqttMock.Object);
        if (set) stateService.MotorMacToConnectionId.TryAdd(dto.Mac, []);
        var handler = new ClientStartsListeningToMotor(stateService);
        var act = async () => await handler.Handle(dto, default);
        await act.Should().NotThrowAsync();
    }

    [Test]
    public void StartUpTest()
    {
        var app = StartupClass.Startup([]);
        app.Should().NotBeNull();
        app.Should().BeOfType<WebApplication>();
    }
}