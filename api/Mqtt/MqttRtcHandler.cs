using api.Utils;
using commons;
using commons.Models;
using MQTTnet;
using Newtonsoft.Json;
using Serilog;
using service;

namespace api.Mqtt;

public class MqttRtcHandler(IWebSocketStateService webSocketStateService)
{
    public async Task CommunicateWithBroker()
    {
        var mqttClient = await MqttClientGenerator.CreateMqttClient("/devices/+/bmedata/rtc");
        mqttClient.ApplicationMessageReceivedAsync += async e =>
        {
            try
            {
                var m = e.ApplicationMessage;
                var message = m.ConvertPayloadToString();
                var mac = m.Topic.Split('/')[2];
                // Log.Debug("Mqtt Message received: {Message}, Topic: {Topic}", message, m.Topic);
                var data = JsonConvert.DeserializeObject<BmeData>(message)!;
                data.CreatedAt = DateTime.UtcNow;
                var dto = new ServerSendsRtcDataDto { Data = data, Mac = mac };
                if (!webSocketStateService.RtcMacToConnectionId.TryGetValue(mac, out var connectionIdList)) return;
                foreach (var connectionId in connectionIdList)
                {
                    if (!webSocketStateService.Connections.TryGetValue(connectionId, out var socket)) continue;
                    try
                    {
                        await socket.SendJson(dto);
                    }
                    catch (Exception exception)
                    {
                        Log.Error(exception, "Error sending message to client");
                    }
                }
            }
            catch (Exception exception)
            {
                Log.Error(exception, "Error handling message");
            }
        };
    }
}

public class ServerSendsRtcDataDto : BaseDto
{
    public required string Mac { get; set; }
    public required BmeData Data { get; set; }
}