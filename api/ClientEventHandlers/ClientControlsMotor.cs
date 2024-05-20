using api.Utils;
using commons;
using MediatR;
using service;

namespace api.ClientEventHandlers;

public class ClientControlsMotorDto : BaseDto, IRequest
{
    public required string Mac { get; set; }
    public required int Position { get; set; }
}

public class ServerSendsMotorDataDto : BaseDto
{
    public required string Mac { get; set; }
    public required int Position { get; set; }
}

public class ClientControlsMotor : IRequestHandler<ClientControlsMotorDto>
{
    public async Task Handle(ClientControlsMotorDto request, CancellationToken cancellationToken)
    {
        var mqttClient = await MqttClientGenerator.CreateMqttClient();
        await mqttClient.PublishJsonAsync($"/devices/{request.Mac}/motor/controls", new { request.Position });
        mqttClient.Dispose();
    }
}

public class ClientStartsListeningToMotorDto : BaseDto, IRequest
{
    public required string Mac { get; set; }
}

public class ClientStartsListeningToMotor(IWebSocketStateService webSocketStateService)
    : IRequestHandler<ClientStartsListeningToMotorDto>
{
    public Task Handle(ClientStartsListeningToMotorDto request, CancellationToken cancellationToken)
    {
        if (webSocketStateService.MotorMacToConnectionId.TryGetValue(request.Mac, out var connectionIdSet))
            connectionIdSet.Add(request.Socket!.ConnectionInfo.Id);
        else
            webSocketStateService.MotorMacToConnectionId.TryAdd(request.Mac, [request.Socket!.ConnectionInfo.Id]);

        return Task.CompletedTask;
    }
}