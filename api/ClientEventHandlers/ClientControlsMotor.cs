using api.Mqtt.Helpers;
using api.Utils;
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

public class ClientControlsMotor(MqttClientGenerator mqttClientGenerator, WebSocketStateService webSocketStateService) : IRequestHandler<ClientControlsMotorDto>
{
    public async Task Handle(ClientControlsMotorDto request, CancellationToken cancellationToken)
    {
        var mqttClient = await mqttClientGenerator.CreateMqttClient();
        await mqttClient.PublishJsonAsync($"/devices/{request.Mac}/motor/controls", new { request.Position });
        if (webSocketStateService.MotorMacToConnectionId.TryGetValue(request.Mac, out var connectionIdList))
        {
            if (!connectionIdList.Contains(request.Socket!.ConnectionInfo.Id))
                connectionIdList.Add(request.Socket!.ConnectionInfo.Id);
        } else
        {
            webSocketStateService.MotorMacToConnectionId.TryAdd(request.Mac, [request.Socket!.ConnectionInfo.Id]);
        }
        
        mqttClient.Dispose();
    }
}