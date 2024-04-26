using api.Mqtt.Helpers;
using api.ServerEvents;
using api.Utils;
using commons.Models;
using infrastructure;
using infrastructure.Models;
using MQTTnet;
using Newtonsoft.Json;
using Serilog;
using service;

namespace api.Mqtt;

public class MqttDeviceDataClient(MqttClientGenerator clientGenerator, DataService dataService, WebSocketStateService webSocketStateService)
{
    public async Task CommunicateWithBroker()
    {
        var mqttClient = await clientGenerator.CreateMqttClient("/data/+/bmedata");
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
                if(webSocketStateService.MacToConnectionId.TryGetValue(mac, out var connectionId))
                {
                    if(webSocketStateService.Connections.TryGetValue(connectionId, out var socket))
                    {
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
            catch (Exception exc)
            {
                Log.Error(exc, "Error handling message");
            }
        };

    }

}