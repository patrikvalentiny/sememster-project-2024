using api.ClientEventHandlers;
using api.Utils;
using commons;
using MQTTnet;
using Newtonsoft.Json;
using Serilog;
using service;

namespace api.Mqtt;

public class MqttDeviceMotorPosition(WebSocketStateService webSocketStateService, MotorService motorService)
{
    public async Task CommunicateWithBroker()
    {
        var mqttClient = await MqttClientGenerator.CreateMqttClient($"/devices/+/motor/data");

        mqttClient.ApplicationMessageReceivedAsync += async e =>
        {
            var m = e.ApplicationMessage;
            var message = m.ConvertPayloadToString();
            var position = JsonConvert.DeserializeObject<ServerSendsMotorDataDto>(message)!.Position;
            var mac = m.Topic.Split('/')[2];
            
            motorService.SetMotorPosition(mac, position);
            
            if(webSocketStateService.MotorMacToConnectionId.TryGetValue(mac, out var connectionIdList))
            {
                foreach (var socketGuid in connectionIdList)
                {
                    if (webSocketStateService.Connections.TryGetValue(socketGuid, out var socket))
                        await socket.SendJson(new ServerSendsMotorDataDto { Mac = mac, Position = position });
                }
            }
        };
    }
}