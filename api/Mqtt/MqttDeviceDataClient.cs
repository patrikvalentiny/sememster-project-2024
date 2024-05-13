using api.ServerEvents;
using api.Utils;
using commons;
using commons.Models;
using infrastructure;
using infrastructure.Models;
using MQTTnet;
using Newtonsoft.Json;
using Serilog;
using service;

namespace api.Mqtt;

public class MqttDeviceDataClient( DataService dataService, WebSocketStateService webSocketStateService)
{
    public async Task CommunicateWithBroker()
    {
        var mqttClient = await MqttClientGenerator.CreateMqttClient("/devices/+/bmedata");
        mqttClient.ApplicationMessageReceivedAsync += async e =>
        {
            try
            {
                var m = e.ApplicationMessage;
                var topic = m.Topic;
                var topicList = topic.Split("/");
                var message = m.ConvertPayloadToString();
                Log.Debug("Mqtt Message received: {Message}, Topic: {Topic}", message, topic);
                var mac = topicList[^2];
                var data = JsonConvert.DeserializeObject<BmeData>(message);
                if (data == null)
                {
                    Log.Error("Error deserializing message");
                    return;
                }

                var insertedData = dataService.InsertData(data, mac);
                Log.Debug("Inserted data: {Data}", JsonConvert.SerializeObject(insertedData));
                await SendDataToClients(mac, insertedData);
            }
            catch (Exception exc)
            {
                Log.Error(exc, "Error handling message");
            }
        };

    }

    private async Task SendDataToClients(string mac, BmeDataDto insertedData)
    {
        if (!webSocketStateService.MacToConnectionId.TryGetValue(mac, out var connectionIdList)) return;
        foreach (var connectionId in connectionIdList)
        {
            if (!webSocketStateService.Connections.TryGetValue(connectionId, out var socket)) continue;
            try
            {
                await socket.SendJson(new ServerDeviceBmeData { Data = insertedData });
            }
            catch (Exception exc)
            {
                Log.Error(exc, "Error sending message to client");
            }
        }
    }
}