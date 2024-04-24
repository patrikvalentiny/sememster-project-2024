using api.Mqtt.Helpers;
using api.ServerEvents;
using api.Utils;
using infrastructure;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Formatter;
using Newtonsoft.Json;
using Serilog;
using service;

namespace api.Mqtt;

public class MqttDevicesClient(WebSocketStateService webSocketStateService, DeviceService deviceService, MqttClientGenerator clientGenerator)
{
    public async Task CommunicateWithBroker()
    {
        var mqttClient = await clientGenerator.CreateMqttClient("/devices");

        mqttClient.ApplicationMessageReceivedAsync += async e =>
        {
            try
            {
                var m = e.ApplicationMessage;
                var topic = m.Topic;
                var message = m.ConvertPayloadToString();
                Log.Debug("Mqtt Message received: {Message}, Topic: {Topic}", message, topic);
                var device = deviceService.InsertDevice(JsonConvert.DeserializeObject<Device>(message)!.Mac);

                webSocketStateService.Connections.Values.ToList().ForEach(async socket =>
                {
                    try
                    {
                        await socket.SendJson(new ServerDeviceOnline{Device = device});
                    }
                    catch (Exception exc)
                    {
                        Log.Error(exc, "Error sending message to client");
                    }
                });
            }
            catch (Exception exc)
            {
                Log.Error(exc, "Error handling message");
            }
        };
    }
}