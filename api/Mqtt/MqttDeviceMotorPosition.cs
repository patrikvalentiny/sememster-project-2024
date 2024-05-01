using api.ClientEventHandlers;
using api.Mqtt.Helpers;
using api.Utils;
using MQTTnet;
using Newtonsoft.Json;
using Serilog;
using service;

namespace api.Mqtt;

public class MqttDeviceMotorPosition(MqttClientGenerator mqttClientGenerator, WebSocketStateService webSocketStateService)
{
    public async Task CommunicateWithBroker()
    {
        var mqttClient = await mqttClientGenerator.CreateMqttClient($"/devices/+/motor/data");

        mqttClient.ApplicationMessageReceivedAsync += async e =>
        {
            var m = e.ApplicationMessage;
            var message = m.ConvertPayloadToString();
            var position = JsonConvert.DeserializeObject<ServerSendsMotorDataDto>(message)!.Position;
            Log.Debug("Mqtt Message received: {Message}, Topic: {Topic}", message, m.Topic);
            var mac = m.Topic.Split('/')[2];
            if(webSocketStateService.MotorMacToConnectionId.TryGetValue(mac, out var connectionIdList))
            {
                foreach (var socketGuid in connectionIdList)
                {
                    if (webSocketStateService.Connections.TryGetValue(socketGuid, out var socket))
                        await socket.SendJson(new ServerSendsMotorDataDto { Mac = mac, Position = position });
                }
                // var socket = webSocketStateService.Connections[connectionId];
                // await socket.SendJson(new ServerSendsMotorDataDto { Mac = m.Topic.Split('/')[2], Position = position });
            }
        };
    }
}